using Microsoft.EntityFrameworkCore;
using MicroServicioTurismo.Models;

namespace MicroServicioTurismo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationDetail> ReservationDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision
            modelBuilder.Entity<Tour>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ReservationDetail>()
                .Property(rd => rd.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // Relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tour>()
                .HasMany(t => t.Reservations)
                .WithOne(r => r.Tour)
                .HasForeignKey(r => r.TourId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.ReservationDetails)
                .WithOne(rd => rd.Reservation)
                .HasForeignKey(rd => rd.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Payments)
                .WithOne(p => p.Reservation)
                .HasForeignKey(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
