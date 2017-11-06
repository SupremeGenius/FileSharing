using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace FileSharing.Persistence.Context
{
    public class ContextFactory
        : IDesignTimeDbContextFactory<DatabaseContext>, IDisposable
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("persistence.json");
            var configuration = configurationBuilder.Build();

            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            var databaseEngine = $"{configuration["DatabaseEngine"]}";
            DatabaseContext context;
            switch (databaseEngine)
            {
                case "SqlServer":
                    builder.UseSqlServer(configuration.GetConnectionString(databaseEngine));
                    context = new DatabaseContext(builder.Options);
                    context.Database.Migrate();
                    break;
                case "PostgreSQL":
                    builder.UseNpgsql(configuration.GetConnectionString(databaseEngine));
                    context = new DatabaseContext(builder.Options);
                    context.Database.Migrate();
                    break;
                default:
                    builder.UseInMemoryDatabase(databaseName: databaseEngine);
                    context = new DatabaseContext(builder.Options);
                    break;
            }
            return context;
        }

        public void Dispose()
        {
        }
    }
}
