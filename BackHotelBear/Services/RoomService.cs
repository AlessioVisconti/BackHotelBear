using BackHotelBear.Models.Data;
using BackHotelBear.Models.Dtos.RoomDtos;
using BackHotelBear.Models.Entity.RoomAndRoomPhotos;
using BackHotelBear.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BackHotelBear.Services
{
    public class RoomService : IRoomService
    {
        private readonly HotelBearDbContext _context;
        private readonly string _uploadsFolder = Path.Combine("wwwroot", "uploads", "rooms");
        public RoomService(HotelBearDbContext context)
        {
            _context = context;
            Directory.CreateDirectory(_uploadsFolder);
        }

        //ROOM
        public async Task<Room> CreateRoomAsync(CreateRoomDto dto)
        {
            var room = new Room
            {
                RoomNumber = dto.RoomNumber,
                RoomName = dto.RoomName,
                Description = dto.Description,
                Beds = dto.Beds,
                BedsTypes = dto.BedsTypes,
                PriceForNight = dto.PriceForNight
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return room;
        }

        public async Task<Room?> UpdateRoomAsync(Guid roomId, UpdateRoomDto dto)
        {
            var room = await _context.Rooms
             .Include(r => r.Photos)
             .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.RoomNumber))
                room.RoomNumber = dto.RoomNumber;
            if (!string.IsNullOrWhiteSpace(dto.RoomName))
                room.RoomName = dto.RoomName;
            if (!string.IsNullOrWhiteSpace(dto.Description))
                room.Description = dto.Description;
            if (dto.Beds.HasValue)
                room.Beds = dto.Beds.Value;
            if (!string.IsNullOrWhiteSpace(dto.BedsTypes))
                room.BedsTypes = dto.BedsTypes;
            if (dto.PriceForNight.HasValue)
                room.PriceForNight = dto.PriceForNight.Value;

            await _context.SaveChangesAsync();
            return room;
        }
        //DELETE(ROOM AND PHOTO)
        public async Task<bool> DeleteRoomAsync(Guid roomId)
        {
            var room = await _context.Rooms
          .Include(r => r.Photos)
          .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null) return false;

            foreach (var photo in room.Photos)
            {
                var filePath = Path.Combine(_uploadsFolder, Path.GetFileName(photo.Url));
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            _context.RoomPhotos.RemoveRange(room.Photos);

            _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<RoomListDto>> GetAllRoomsAsync()
        {
            return await _context.Rooms
            .Include(r => r.Photos)
            .Select(r => new RoomListDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                RoomName = r.RoomName,
                Description = r.Description,
                Beds = r.Beds,
                BedsTypes = r.BedsTypes,
                PriceForNight = r.PriceForNight,
                CoverPhotoUrl = r.Photos.Where(p => p.IsCover).Select(p => p.Url).FirstOrDefault()
            })
            .ToListAsync();
        }

        public async Task<RoomDetailDto?> GetRoomDetailAsync(Guid roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Photos)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null) return null;

            return new RoomDetailDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RoomName = room.RoomName,
                Description = room.Description,
                Beds = room.Beds,
                BedsTypes = room.BedsTypes,
                PriceForNight = room.PriceForNight,
                Photos = room.Photos.Select(p => new RoomPhotoDto
                {
                    Id = p.Id,
                    Url = p.Url,
                    IsCover = p.IsCover
                }).ToList()
            };
        }

        public async Task<List<RoomCalendarDto>> GetRoomCalendarAsync(
    DateTime? startDate = null,
    DateTime? endDate = null)
        {
            var start = startDate?.Date ?? DateTime.Today;
            var end = endDate?.Date ?? start.AddDays(15);

            if (end < start)
                throw new ArgumentException("endDate must be >= startDate");

            var rooms = await _context.Rooms
                .Select(r => new
                {
                    r.Id,
                    r.RoomNumber,
                    r.RoomName,
                    r.PriceForNight
                })
                .ToListAsync();

            var reservations = await _context.Reservations
                .Where(r =>
                    r.DeletedAt == null &&
                    r.CheckOut > start &&
                    r.CheckIn < end)
                .Select(r => new
                {
                    r.Id,
                    r.RoomId,
                    r.CheckIn,
                    r.CheckOut,
                    r.FirstName,
                    r.LastName,
                    r.Status
                })
                .ToListAsync();

            var reservationsByRoom = reservations
                .GroupBy(r => r.RoomId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = rooms.Select(room =>
            {
                reservationsByRoom.TryGetValue(room.Id, out var roomReservations);

                return new RoomCalendarDto
                {
                    RoomId = room.Id,
                    RoomNumber = room.RoomNumber,
                    RoomName= room.RoomName,
                    RoomPrice=room.PriceForNight,
                    Reservations = roomReservations?
                        .Select(r => new ReservationBarDto
                        {
                            ReservationId = r.Id,
                            CheckIn = r.CheckIn,
                            CheckOut = r.CheckOut,
                            GuestName = $"{r.FirstName} {r.LastName}",
                            Status = r.Status,
                            StartsBeforeRange = r.CheckIn < start,
                            EndsAfterRange = r.CheckOut > end
                        })
                        .OrderBy(r => r.CheckIn)
                        .ToList() ?? new()
                };
            }).ToList();

            return result;
        }

        public async Task<List<RoomListDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            checkIn = checkIn.Date;
            checkOut = checkOut.Date;

            // find occupied rooms in range
            var occupiedRoomIds = await _context.Reservations
                .Where(r =>
                    r.DeletedAt == null &&
                    r.CheckIn < checkOut &&
                    r.CheckOut > checkIn
                )
                .Select(r => r.RoomId)
                .Distinct()
                .ToListAsync();

            // returns only those not occupied
            return await _context.Rooms
                .Include(r => r.Photos)
                .Where(r => !occupiedRoomIds.Contains(r.Id))
                .Select(r => new RoomListDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomName = r.RoomName,
                    Description = r.Description,
                    Beds = r.Beds,
                    BedsTypes = r.BedsTypes,
                    PriceForNight = r.PriceForNight,
                    CoverPhotoUrl = r.Photos
                        .Where(p => p.IsCover)
                        .Select(p => p.Url)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<List<RoomAvailabilityDto>> GetRoomOccupiedDatesAsync(Guid roomId)
        {
            return await _context.Reservations
            .Where(r => r.RoomId == roomId && r.DeletedAt == null)
            .Select(r => new RoomAvailabilityDto
            {
                CheckIn = r.CheckIn,
                CheckOut = r.CheckOut
            })
            .ToListAsync();
        }
        
        //PHOTO
        public async Task AddRoomPhotoAsync(Guid roomId, AddRoomPhotoDto dto)
        {
            var room = await _context.Rooms
            .Include(r => r.Photos)
            .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
                throw new Exception("Room not found");

            // check file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                throw new Exception("File type not allowed");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(_uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var photoUrl = $"/uploads/rooms/{fileName}";

            if (dto.IsCover)
                foreach (var p in room.Photos)
                    p.IsCover = false;

            var photo = new RoomPhoto
            {
                RoomId = roomId,
                Url = photoUrl,
                IsCover = dto.IsCover
            };

            room.Photos.Add(photo);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SetCoverPhotoAsync(Guid photoId)
        {
            var photo = await _context.RoomPhotos
                .Include(p => p.Room)
                .ThenInclude(r => r.Photos)
                .FirstOrDefaultAsync(p => p.Id == photoId);

            if (photo == null) return false;

            foreach (var p in photo.Room.Photos)
                p.IsCover = false;

            photo.IsCover = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoomPhotoAsync(Guid photoId)
        {
            var photo = await _context.RoomPhotos.FindAsync(photoId);
            if (photo == null) return false;

            var filePath = Path.Combine(_uploadsFolder, Path.GetFileName(photo.Url));
            if (File.Exists(filePath))
                File.Delete(filePath);

            _context.RoomPhotos.Remove(photo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
