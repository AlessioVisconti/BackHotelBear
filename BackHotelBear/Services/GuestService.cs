using BackHotelBear.Models.Data;
using BackHotelBear.Models.Dtos.GuestDtos;
using BackHotelBear.Models.Entity.GuestAndEnum;
using BackHotelBear.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BackHotelBear.Services
{
    public class GuestService : IGuestService
    {
        private readonly HotelBearDbContext _context;
        public GuestService(HotelBearDbContext context)
        {
            _context = context;
        }
        public async Task<GuestResult> CreateGuestAsync(GuestDto dto)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == dto.ReservationId && r.DeletedAt == null);

            if (reservation == null)
                return new GuestResult { Success = false, ErrorMessage = "Reservation not found" };
            try
            {
                ValidateGuestDto(dto, reservation.Id);
            }
            catch (ArgumentException ex)
            {
                return new GuestResult { Success = false, ErrorMessage = ex.Message };
            }

            var guest = new Guest
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                BirthCity = dto.BirthCity,
                Citizenship = dto.Citizenship,
                Role = Enum.Parse<GuestRole>(dto.Role),
                TaxCode = dto.TaxCode,
                Address = dto.Address,
                CityOfResidence = dto.CityOfResidence,
                Province = dto.Province,
                PostalCode = dto.PostalCode,
                DocumentType = dto.DocumentType,
                DocumentNumber = dto.DocumentNumber,
                DocumentExpiration = dto.DocumentExpiration,
                ReservationId = dto.ReservationId,
                CreatedBy = dto.CreatedBy ?? "System"
            };

            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            return new GuestResult { Success = true, Guest = await MapToDto(guest) };
        }
        public async Task<GuestResult> UpdateGuestAsync(Guid guestId, GuestDto dto)
        {
            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Id == guestId && g.DeletedAt == null);
            if (guest == null)
                return new GuestResult { Success = false, ErrorMessage = "Guest not found" };

            guest.FirstName = dto.FirstName ?? guest.FirstName;
            guest.LastName = dto.LastName ?? guest.LastName;
            guest.BirthDate = dto.BirthDate != default ? dto.BirthDate : guest.BirthDate;
            guest.BirthCity = dto.BirthCity ?? guest.BirthCity;
            guest.Citizenship = dto.Citizenship ?? guest.Citizenship;
            if (!string.IsNullOrWhiteSpace(dto.Role)) guest.Role = Enum.Parse<GuestRole>(dto.Role);
            guest.TaxCode = dto.TaxCode ?? guest.TaxCode;
            guest.Address = dto.Address ?? guest.Address;
            guest.CityOfResidence = dto.CityOfResidence ?? guest.CityOfResidence;
            guest.Province = dto.Province ?? guest.Province;
            guest.PostalCode = dto.PostalCode ?? guest.PostalCode;
            guest.DocumentType = dto.DocumentType ?? guest.DocumentType;
            guest.DocumentNumber = dto.DocumentNumber ?? guest.DocumentNumber;
            guest.DocumentExpiration = dto.DocumentExpiration ?? guest.DocumentExpiration;

            guest.UpdatedAt = DateTime.UtcNow;
            guest.UpdatedBy = dto.UpdatedBy ?? "System";

            try
            {
                ValidateGuestDto(dto, guest.ReservationId);
            }
            catch (ArgumentException ex)
            {
                return new GuestResult { Success = false, ErrorMessage = ex.Message };
            }

            await _context.SaveChangesAsync();

            return new GuestResult { Success = true, Guest = await MapToDto(guest) };
        }

        public async Task<GuestResult> DeleteGuestAsync(Guid guestId)
        {
            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Id == guestId && g.DeletedAt == null);
            if (guest == null)
                return new GuestResult { Success = false, ErrorMessage = "Guest not found" };

            guest.DeletedAt = DateTime.UtcNow;
            guest.DeletedBy = "System";

            await _context.SaveChangesAsync();
            return new GuestResult { Success = true, Guest = await MapToDto(guest) };
        }

        public async Task<List<GuestDto>> SearchGuestAsync(GuestResearchDto dto)
        {
            var query = _context.Guests
                .Where(g => g.DeletedAt == null);
            if (!string.IsNullOrWhiteSpace(dto.Name))
                query = query.Where(g => g.FirstName.Contains(dto.Name) || g.LastName.Contains(dto.Name));
            return await query
                .Select(g => new GuestDto
                {
                    Id = g.Id,
                    FirstName = g.FirstName,
                    LastName = g.LastName,
                    BirthDate = g.BirthDate,
                    BirthCity = g.BirthCity,
                    Citizenship = g.Citizenship,
                    Role = g.Role.ToString(),
                    TaxCode = g.TaxCode,
                    Address = g.Address,
                    CityOfResidence = g.CityOfResidence,
                    Province = g.Province,
                    PostalCode = g.PostalCode,
                    DocumentType = g.DocumentType,
                    DocumentNumber = g.DocumentNumber,
                    DocumentExpiration = g.DocumentExpiration,
                    ReservationId = g.ReservationId
                })
                .ToListAsync();
        }


        private async Task<GuestDto> MapToDto(Guest guest)
        {
            async Task<string?> ResolveUserAsync(string? userId)
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return null;

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                return user != null ? user.FullName : userId;
            }

            return new GuestDto
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                BirthDate = guest.BirthDate,
                BirthCity = guest.BirthCity,
                Citizenship = guest.Citizenship,
                Role = guest.Role.ToString(),
                TaxCode = guest.TaxCode,
                Address = guest.Address,
                CityOfResidence = guest.CityOfResidence,
                Province = guest.Province,
                PostalCode = guest.PostalCode,
                DocumentType = guest.DocumentType,
                DocumentNumber = guest.DocumentNumber,
                DocumentExpiration = guest.DocumentExpiration,
                ReservationId = guest.ReservationId,

                CreatedBy = await ResolveUserAsync(guest.CreatedBy),
                CreatedAt = guest.CreatedAt,
                UpdatedBy = await ResolveUserAsync(guest.UpdatedBy),
                UpdatedAt = guest.UpdatedAt,
                DeletedBy = await ResolveUserAsync(guest.DeletedBy),
                DeletedAt = guest.DeletedAt
            };
        }

         private void ValidateGuestDto(GuestDto dto, Guid reservationId)
        {
            if (!Enum.TryParse<GuestRole>(dto.Role, out var role)) return;
            if (role == GuestRole.Single || role == GuestRole.HeadOfFamily || role == GuestRole.GroupLeader)
            {
                if (string.IsNullOrWhiteSpace(dto.TaxCode) ||
                    string.IsNullOrWhiteSpace(dto.Address) ||
                    string.IsNullOrWhiteSpace(dto.CityOfResidence) ||
                    string.IsNullOrWhiteSpace(dto.PostalCode) ||
                    dto.DocumentType == null ||
                    string.IsNullOrWhiteSpace(dto.DocumentNumber) ||
                    !dto.DocumentExpiration.HasValue)
                {
                    throw new ArgumentException($"Guest with role {role} must have all mandatory details filled.");
                }

                if (role == GuestRole.FamilyMember)
                {
                    var head = _context.Guests.FirstOrDefault(g =>
                        g.ReservationId == reservationId &&
                        g.Role == GuestRole.HeadOfFamily &&
                        g.DeletedAt == null);

                    if (head == null)
                        throw new ArgumentException("FamilyMember must belong to a reservation with a HeadOfFamily.");
                }

                if (role == GuestRole.GroupMember)
                {
                    var leader = _context.Guests.FirstOrDefault(g =>
                        g.ReservationId == reservationId &&
                        g.Role == GuestRole.GroupLeader &&
                        g.DeletedAt == null);

                    if (leader == null)
                        throw new ArgumentException("GroupMember must belong to a reservation with a GroupLeader.");
                }
            }

         }
    }
}

