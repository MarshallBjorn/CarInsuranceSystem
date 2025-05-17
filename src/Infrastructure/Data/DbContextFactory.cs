using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>, IDbContextFactory<AppDbContext>
    {
        private readonly IConfiguration _configuration;

        // For runtime use (Avalonia)
        public AppDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // For design-time use (migrations)
        public AppDbContextFactory()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public AppDbContext CreateDbContext()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString, o => o.UseNodaTime());

            return new AppDbContext(optionsBuilder.Options);
        }

        // For IDesignTimeDbContextFactory
        public AppDbContext CreateDbContext(string[] args)
        {
            return CreateDbContext();
        }
    }
}