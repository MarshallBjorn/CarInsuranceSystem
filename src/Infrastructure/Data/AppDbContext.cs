using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Firm> Firms { get; set; }
        public DbSet<UserCar> UserCars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasKey(c => c.VIN);  // VIN is the primary key

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);  // Assuming User has an Id property

            modelBuilder.Entity<Insurance>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Firm>()
                .HasKey(f => f.Id);
            // Configure UserCar (N:M) composite key
            modelBuilder.Entity<UserCar>()
                .HasKey(uc => new { uc.UserId, uc.CarVIN });

            // Configure 1:N (Insurance → Car)
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Insurance)
                .WithMany(i => i.Cars)
                .HasForeignKey(c => c.InsuranceId)
                .OnDelete(DeleteBehavior.SetNull);  // Optional relationship

            // Configure 1:N (Firm → Insurance)
            modelBuilder.Entity<Insurance>()
                .HasOne(i => i.Firm)
                .WithMany(f => f.Insurances)
                .HasForeignKey(i => i.FirmId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}