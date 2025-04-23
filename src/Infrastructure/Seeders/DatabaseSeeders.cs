using Core.Entities;
using Infrastructure.Data;

namespace Infrastructure.Seeders;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context) 
    {
        if (!context.Firms.Any())
        {
            var firms = new List<Firm>
            {
                new() { 
                    Id = Guid.NewGuid(),
                    Name = "Black FOX", 
                    CountryCode = "UA",
                    CreatedAt = DateTime.UtcNow
                },
                new() { 
                    Id = Guid.NewGuid(),
                    Name = "Tegoniemasz Industries", 
                    CountryCode = "PL",
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Firms.AddRange(firms);
            context.SaveChanges();
        }

        if (!context.Insurances.Any())
        {
            var fox = context.Firms.First(f => f.Name == "Black FOX");
            var tegnie = context.Firms.First(f => f.Name == "Tegoniemasz Industries");

            var insurances = new List<Insurance>
            {
                new() {
                    Id = Guid.NewGuid(),
                    PolicyNumber = "POL-ALL-123",
                    Type = "Premium",
                    FirmId = fox.Id,
                    ValidFrom = DateTime.UtcNow.AddYears(-1),
                    ValidTo = DateTime.UtcNow.AddYears(1),
                    IsActive = true
                },
                new() {
                    Id = Guid.NewGuid(),
                    PolicyNumber = "POL-SF-456",
                    Type = "Basic",
                    FirmId = tegnie.Id,
                    ValidFrom = DateTime.UtcNow,
                    ValidTo = DateTime.UtcNow.AddYears(2),
                    IsActive = true
                }
            };
            context.Insurances.AddRange(insurances);
            context.SaveChanges();
        }

        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new () { 
                    Id = Guid.NewGuid(),
                    Email = "oleksij.nawrockij@gmail.com", 
                    FirstName = "Oleksij", 
                    LastName = "Nawrockij", 
                    BirthDate = DateTime.SpecifyKind(new DateTime(2004, 12, 8), DateTimeKind.Utc)
                },
                new () {
                    Id = Guid.NewGuid(),
                    Email = "tomasz.nowak@gmail.com",
                    FirstName = "Tomasz",
                    LastName = "Nowak",
                    BirthDate = DateTime.SpecifyKind(new DateTime(2003, 11, 2), DateTimeKind.Utc)
                }
            };
            
            context.Users.AddRange(users);
            context.SaveChanges();
        }

        if (!context.Cars.Any())
        {
            var premiumInsurance = context.Insurances.First(i => i.PolicyNumber == "POL-ALL-123");

            var cars = new List<Car>
            {
                new() { 
                    VIN = "JM1TA221321173708", 
                    Mark = "Mazda", 
                    Model = "3", 
                    ProductionYear = 2012,
                    EngineType = "Petrol",
                    InsuranceId = premiumInsurance.Id
                },
                new() {
                    VIN = "WMEEK8AA9BK479016", 
                    Mark = "Seat", 
                    Model = "Leon", 
                    ProductionYear = 2008,
                    EngineType = "Diesel",
                    InsuranceId = premiumInsurance.Id
                }
            };

            context.Cars.AddRange(cars);
            context.SaveChanges();
        }

        if(!context.UserCars.Any())
        {
            var mazda = context.Cars.First(i => i.Mark == "Mazda");
            var leonidas = context.Cars.First(i => i.Mark == "Seat");
            var oleksij = context.Users.First(i => i.FirstName == "Oleksij");
            var tomeg = context.Users.First(i => i.FirstName == "Tomasz");

            var userCars = new List<UserCar>
            {
                new() {
                    UserId = oleksij.Id,
                    CarVIN = mazda.VIN,
                    PurchaseDate = DateTime.UtcNow.AddMonths(-6),
                    IsCurrentOwner = true
                },
                new () {
                    UserId = tomeg.Id,
                    CarVIN = leonidas.VIN,
                    PurchaseDate = DateTime.UtcNow.AddMonths(-2),
                    IsCurrentOwner = true
                }
            };

            context.UserCars.AddRange(userCars);
            context.SaveChanges();
        }
    }
}