using Bogus;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Ensure database exists
        await context.Database.EnsureCreatedAsync();

        var utcNow = DateTime.UtcNow;

        // Seed Users
        List<User> users;
        if (!await context.Users.AnyAsync())
        {
            var seededUsers = new List<User>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Email = "oleksij.nawrockij@gmail.com",
                    FirstName = "Oleksij",
                    LastName = "Nawrockij",
                    BirthDate = new DateTime(2004, 12, 8).ToUniversalTime(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("supperp@sswrd123"),
                    Role = "User"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Email = "tomasz.nowak@gmail.com",
                    FirstName = "Tomasz",
                    LastName = "Nowak",
                    BirthDate = new DateTime(2003, 11, 2).ToUniversalTime(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("321"),
                    Role = "User"
                }
            };

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Id, _ => Guid.NewGuid())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.BirthDate, f => f.Date.Past(30, utcNow.AddYears(-18)))
                .RuleFor(u => u.PasswordHash, _ => BCrypt.Net.BCrypt.HashPassword("password"))
                .RuleFor(u => u.Role, _ => "User");

            users = seededUsers.Concat(userFaker.Generate(40)).ToList();

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
        else
        {
            users = await context.Users.ToListAsync();
        }

        var oleksij = users.FirstOrDefault(u => u.FirstName == "Oleksij");
        var tomasz = users.FirstOrDefault(u => u.FirstName == "Tomasz");

        if (oleksij is null || tomasz is null)
            throw new Exception("Seed users 'Oleksij' or 'Tomasz' not found.");

        // Seed Firms
        List<Firm> firms;
        if (!await context.Firms.AnyAsync())
        {
            var seededFirms = new List<Firm>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Black FOX",
                    CountryCode = "UA",
                    CreatedAt = utcNow,
                    UserId = oleksij.Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Tegoniemasz Industries",
                    CountryCode = "PL",
                    CreatedAt = utcNow,
                    UserId = tomasz.Id
                }
            };

            var firmFaker = new Faker<Firm>()
                .RuleFor(f => f.Id, _ => Guid.NewGuid())
                .RuleFor(f => f.Name, f => f.Company.CompanyName())
                .RuleFor(f => f.CountryCode, f => f.Address.CountryCode())
                .RuleFor(f => f.CreatedAt, f => f.Date.Past(10, utcNow))
                .RuleFor(f => f.UserId, f => f.PickRandom(users).Id);

            firms = seededFirms.Concat(firmFaker.Generate(60)).ToList();

            await context.Firms.AddRangeAsync(firms);
            await context.SaveChangesAsync();
        }
        else
        {
            firms = await context.Firms.ToListAsync();
        }

        var fox = firms.FirstOrDefault(f => f.Name == "Black FOX");
        var tegnie = firms.FirstOrDefault(f => f.Name == "Tegoniemasz Industries");

        if (fox is null || tegnie is null)
            throw new Exception("Seed firms not found.");

        // Seed InsuranceTypes
        List<InsuranceType> insurances;
        if (!await context.InsuranceTypes.AnyAsync())
        {
            var seededInsuranceTypes = new List<InsuranceType>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Premium",
                    PolicyDescription = "Premium insurance policy",
                    PolicyNumber = "POL-ALL-123",
                    Price = 1200,
                    FirmId = fox.Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Basic",
                    PolicyDescription = "Basic insurance policy",
                    PolicyNumber = "POL-SF-456",
                    Price = 900,
                    FirmId = tegnie.Id
                }
            };

            var insuranceFaker = new Faker<InsuranceType>()
                .RuleFor(i => i.Id, _ => Guid.NewGuid())
                .RuleFor(i => i.Name, f => f.PickRandom("Basic", "Premium", "Advanced"))
                .RuleFor(i => i.PolicyDescription, f => f.Lorem.Sentence())
                .RuleFor(i => i.PolicyNumber, f => $"POL-{f.Random.AlphaNumeric(7).ToUpper()}")
                .RuleFor(i => i.Price, f => Math.Round(f.Random.Decimal(100, 1000), 2))
                .RuleFor(i => i.FirmId, f => f.PickRandom(firms).Id);

            insurances = seededInsuranceTypes.Concat(insuranceFaker.Generate(100)).ToList();

            await context.InsuranceTypes.AddRangeAsync(insurances);
            await context.SaveChangesAsync();
        }
        else
        {
            insurances = await context.InsuranceTypes.ToListAsync();
        }

        // Seed Cars
        List<Car> cars;
        if (!await context.Cars.AnyAsync())
        {
            var seededCars = new List<Car>
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

            var carFaker = new Faker<Car>()
                .RuleFor(c => c.VIN, f => f.Random.Replace("??######????????"))
                .RuleFor(c => c.Mark, f => f.Vehicle.Manufacturer())
                .RuleFor(c => c.Model, f => f.Vehicle.Model())
                .RuleFor(c => c.ProductionYear, f => f.Date.Past(20).Year)
                .RuleFor(c => c.EngineType, f => f.PickRandom("Petrol", "Diesel", "Hybrid", "Electric"));

            cars = seededCars.Concat(carFaker.Generate(500)).ToList();

            await context.Cars.AddRangeAsync(cars);
            await context.SaveChangesAsync();
        }
        else
        {
            cars = await context.Cars.ToListAsync();
        }

        var mazda = cars.FirstOrDefault(c => c.VIN == "JM1TA221321173708");
        var seat = cars.FirstOrDefault(c => c.VIN == "WMEEK8AA9BK479016");
        var premium = insurances.FirstOrDefault(i => i.PolicyNumber == "POL-ALL-123");
        var basic = insurances.FirstOrDefault(i => i.PolicyNumber == "POL-SF-456");

        // Seed CarInsurances
        if (!await context.CarInsurances.AnyAsync())
        {
            if (mazda is null || seat is null || premium is null || basic is null)
                throw new Exception("Required seeded data for car insurances not found.");

            var seededCarInsurances = new List<CarInsurance>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CarVIN = mazda.VIN,
                    InsuranceTypeId = premium.Id,
                    ValidFrom = utcNow.AddYears(-1),
                    ValidTo = utcNow.AddYears(1),
                    IsActive = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CarVIN = seat.VIN,
                    InsuranceTypeId = basic.Id,
                    ValidFrom = utcNow,
                    ValidTo = utcNow.AddYears(2),
                    IsActive = true
                }
            };

            var carInsuranceFaker = new Faker<CarInsurance>()
                .RuleFor(ci => ci.Id, _ => Guid.NewGuid())
                .RuleFor(ci => ci.CarVIN, f => f.PickRandom(cars).VIN)
                .RuleFor(ci => ci.InsuranceTypeId, f => f.PickRandom(insurances).Id)
                .RuleFor(ci => ci.ValidFrom, f => f.Date.Past(1, utcNow))
                .RuleFor(ci => ci.ValidTo, (f, ci) => ci.ValidFrom.AddYears(1))
                .RuleFor(ci => ci.IsActive, _ => true);

            await context.CarInsurances.AddRangeAsync(seededCarInsurances);
            await context.CarInsurances.AddRangeAsync(carInsuranceFaker.Generate(500));
            await context.SaveChangesAsync();
        }

        if (!await context.UserCars.AnyAsync())
        {
            if (mazda is null || seat is null)
                throw new Exception("Required seeded cars not found for UserCars.");

            var seededUserCars = new List<UserCar>
            {
                new()
                {
                    UserId = oleksij.Id,
                    CarVIN = mazda.VIN,
                    PurchaseDate = utcNow.AddMonths(-6),
                    IsCurrentOwner = true
                },
                new()
                {
                    UserId = tomasz.Id,
                    CarVIN = seat.VIN,
                    PurchaseDate = utcNow.AddMonths(-2),
                    IsCurrentOwner = true
                }
            };

            var usedKeys = new HashSet<(Guid, string)>
            {
                (oleksij.Id, mazda.VIN),
                (tomasz.Id, seat.VIN)
            };

            var userCarsToAdd = new List<UserCar>(seededUserCars);
            var faker = new Faker();

            int attempts = 0;
            while (userCarsToAdd.Count < 12 && attempts < 100)
            {
                attempts++;

                var userId = faker.PickRandom(users).Id;
                var carVIN = faker.PickRandom(cars).VIN;

                if (usedKeys.Contains((userId, carVIN)))
                    continue;

                usedKeys.Add((userId, carVIN));

                userCarsToAdd.Add(new UserCar
                {
                    UserId = userId,
                    CarVIN = carVIN,
                    PurchaseDate = faker.Date.Past(3, utcNow),
                    IsCurrentOwner = faker.Random.Bool()
                });
            }

            await context.UserCars.AddRangeAsync(userCarsToAdd);
            await context.SaveChangesAsync();
        }
    }
}
