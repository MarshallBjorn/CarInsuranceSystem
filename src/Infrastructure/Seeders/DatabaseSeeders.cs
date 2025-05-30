using Core.Entities;
using Infrastructure.Data;

namespace Infrastructure.Seeders;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Email = "oleksij.nawrockij@gmail.com",
                    FirstName = "Oleksij",
                    LastName = "Nawrockij",
                    BirthDate = DateTime.SpecifyKind(new DateTime(2004, 12, 8), DateTimeKind.Utc),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123"),
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Email = "tomasz.nowak@gmail.com",
                    FirstName = "Tomasz",
                    LastName = "Nowak",
                    BirthDate = DateTime.SpecifyKind(new DateTime(2003, 11, 2), DateTimeKind.Utc),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("321"),
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        // Retrieve the users after ensuring they are saved
        var oleksij = context.Users.First(u => u.FirstName == "Oleksij");
        var tomasz = context.Users.First(u => u.FirstName == "Tomasz");

        if (!context.Firms.Any())
        {
            var firms = new List<Firm>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Black FOX",
                    CountryCode = "UA",
                    CreatedAt = DateTime.UtcNow,
                    UserId = oleksij.Id // 👈 Assign Oleksij as owner
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Tegoniemasz Industries",
                    CountryCode = "PL",
                    CreatedAt = DateTime.UtcNow,
                    UserId = tomasz.Id // 👈 Assign Tomasz as owner
                }
            };

            context.Firms.AddRange(firms);
            context.SaveChanges();
        }

        if (!context.InsuranceTypes.Any())
        {
            var fox = context.Firms.First(f => f.Name == "Black FOX");
            var tegnie = context.Firms.First(f => f.Name == "Tegoniemasz Industries");

            var insuranceTypes = new List<InsuranceType>
            {
                new() {
                    Id = Guid.NewGuid(),
                    Name = "Premium",
                    PolicyDescription = "Premium insurance policy",
                    PolicyNumber = "POL-ALL-123",
                    FirmId = fox.Id
                },
                new() {
                    Id = Guid.NewGuid(),
                    Name = "Basic",
                    PolicyDescription = "Basic insurance policy",
                    PolicyNumber = "POL-SF-456",
                    FirmId = tegnie.Id
                }
            };

            context.InsuranceTypes.AddRange(insuranceTypes);
            context.SaveChanges();
        }

        if (!context.Cars.Any())
        {
            var cars = new List<Car>
            {
                new()
                {
                    VIN = "JM1TA221321173708",
                    Mark = "Mazda",
                    Model = "3",
                    ProductionYear = 2012,
                    EngineType = "Petrol"
                },
                new()
                {
                    VIN = "WMEEK8AA9BK479016",
                    Mark = "Seat",
                    Model = "Leon",
                    ProductionYear = 2008,
                    EngineType = "Diesel"
                }
            };

            context.Cars.AddRange(cars);
            context.SaveChanges();
        }

        if (!context.CarInsurances.Any())
        {
            var premiumInsuranceType = context.InsuranceTypes.First(i => i.PolicyNumber == "POL-ALL-123");
            var basicInsuranceType = context.InsuranceTypes.First(i => i.PolicyNumber == "POL-SF-456");

            var mazda = context.Cars.First(c => c.VIN == "JM1TA221321173708");
            var seat = context.Cars.First(c => c.VIN == "WMEEK8AA9BK479016");

            var carInsurances = new List<CarInsurance>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CarVIN = mazda.VIN,
                    InsuranceTypeId = premiumInsuranceType.Id,
                    ValidFrom = DateTime.UtcNow.AddYears(-1),
                    ValidTo = DateTime.UtcNow.AddYears(1),
                    IsActive = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CarVIN = seat.VIN,
                    InsuranceTypeId = basicInsuranceType.Id,
                    ValidFrom = DateTime.UtcNow,
                    ValidTo = DateTime.UtcNow.AddYears(2),
                    IsActive = true
                }
            };

            context.CarInsurances.AddRange(carInsurances);
            context.SaveChanges();
        }

        if (!context.UserCars.Any())
        {
            var mazda = context.Cars.First(c => c.Mark == "Mazda");
            var seat = context.Cars.First(c => c.Mark == "Seat");

            var userCars = new List<UserCar>
            {
                new()
                {
                    UserId = oleksij.Id,
                    CarVIN = mazda.VIN,
                    PurchaseDate = DateTime.UtcNow.AddMonths(-6),
                    IsCurrentOwner = true
                },
                new()
                {
                    UserId = tomasz.Id,
                    CarVIN = seat.VIN,
                    PurchaseDate = DateTime.UtcNow.AddMonths(-2),
                    IsCurrentOwner = true
                }
            };

            context.UserCars.AddRange(userCars);
            context.SaveChanges();
        }
    }
}
