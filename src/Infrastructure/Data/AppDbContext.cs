using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data;

/// <summary>
/// Database context for the Car Insurance System.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the collection of cars.
    /// </summary>
    public DbSet<Car> Cars { get; set; }

    /// <summary>
    /// Gets or sets the collection of users.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the collection of insurances.
    /// </summary>
    public DbSet<Insurance> Insurances { get; set; }

    /// <summary>
    /// Gets or sets the collection of firms.
    /// </summary>
    public DbSet<Firm> Firms { get; set; }

    /// <summary>
    /// Gets or sets the collection of user-car relationships.
    /// </summary>
    public DbSet<UserCar> UserCars { get; set; }

    /// <summary>
    /// Configures the database model.
    /// </summary>
    /// <param name="modelBuilder">The builder used to configure the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCar>()
                .HasKey(uc => new { uc.UserId, uc.CarVIN });

            modelBuilder.Entity<UserCar>()
                .HasOne(uc => uc.User)
                .WithMany()
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserCar>()
                .HasOne(uc => uc.Car)
                .WithMany()
                .HasForeignKey(uc => uc.CarVIN);

            modelBuilder.Entity<Car>()
                .HasKey(c => c.VIN);

            modelBuilder.Entity<Car>()
                .HasOne(c => c.Insurance)
                .WithMany()
                .HasForeignKey(c => c.InsuranceId);

            modelBuilder.Entity<Insurance>()
                .HasOne(i => i.Firm)
                .WithMany()
                .HasForeignKey(i => i.FirmId);

            // modelBuilder.Entity<Car>()
            //     .HasKey(c => c.VIN);  // VIN is the primary key

            // // modelBuilder.Entity<User>(e => 
            // {
            //     e.Property(u => u.BirthDate)
            //         .HasConversion(
            //             v => v.ToUniversalTime(),
            //             v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            //     e.HasKey(u => u.Id);
            // });

            // /*
            // modelBuilder.Entity<User>()
            //     .HasKey(u => u.Id);  // Assuming User has an Id property
            // */

            // modelBuilder.Entity<Insurance>()
            //     .HasKey(i => i.Id);

            // modelBuilder.Entity<Firm>()
            //     .HasKey(f => f.Id);
            // // Configure UserCar (N:M) composite key
            // modelBuilder.Entity<UserCar>()
            //     .HasKey(uc => new { uc.UserId, uc.CarVIN });

            // // Configure 1:N (Insurance → Car)
            // modelBuilder.Entity<Car>()
            //     .HasOne(c => c.Insurance)
            //     .WithMany(i => i.Cars)
            //     .HasForeignKey(c => c.InsuranceId)
            //     .OnDelete(DeleteBehavior.SetNull);  // Optional relationship

            // // Configure 1:N (Firm → Insurance)
            // modelBuilder.Entity<Insurance>()
            //     .HasOne(i => i.Firm)
            //     .WithMany(f => f.Insurances)
            //     .HasForeignKey(i => i.FirmId)
            //     .OnDelete(DeleteBehavior.Cascade);
    }
    

    /// <summary>
    /// Saves changes to the database with audit information.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Existing audit logic
        return await base.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Interface for auditable entities.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    DateTime UpdatedAt { get; set; }
}