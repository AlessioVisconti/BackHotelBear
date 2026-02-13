using BackHotelBear.Models.Entity;
using BackHotelBear.Models.Entity.ChargeAndEnum;
using BackHotelBear.Models.Entity.GuestAndEnum;
using BackHotelBear.Models.Entity.InvoiceAndEnum;
using BackHotelBear.Models.Entity.PaymentAndEnum;
using BackHotelBear.Models.Entity.ReservationAndEnum;
using BackHotelBear.Models.Entity.RoomAndRoomPhotos;
using BackHotelBear.Services.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// per gli enum EDocumentType EInvoice e così via
namespace BackHotelBear.Models.Data
{
    public class HotelBearDbContext : IdentityDbContext<User>
    {
        private readonly ICurrentUserService _currentUserService;
        public HotelBearDbContext(DbContextOptions<HotelBearDbContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomPhoto> RoomPhotos { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Charge> Charges { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<InvoicePayment> InvoicePayments { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            string? userId = _currentUserService.UserId ?? "system";
            foreach(var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = userId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy = userId;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        entry.Entity.DeletedBy = userId;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
        public async Task<int> HardDeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            
            Entry(entity).State = EntityState.Deleted;

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Reservation)
                .WithMany(r => r.Invoices)
                .HasForeignKey(i => i.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithOne(c => c.Invoice)
                .HasForeignKey<InvoiceCustomer>(c => c.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<InvoicePayment>()
                .HasKey(ip => new { ip.InvoiceId, ip.PaymentId });

            modelBuilder.Entity<InvoicePayment>()
                .HasOne(ip => ip.Invoice)
                .WithMany(i => i.InvoicePayments)
                .HasForeignKey(ip => ip.InvoiceId);

            modelBuilder.Entity<InvoicePayment>()
                .HasOne(ip => ip.Payment)
                .WithMany()
                .HasForeignKey(ip => ip.PaymentId);
        }
    }
}
