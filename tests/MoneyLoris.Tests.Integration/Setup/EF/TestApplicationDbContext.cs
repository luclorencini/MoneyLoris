using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoneyLoris.Infrastructure.Persistence.Context;

namespace MoneyLoris.Tests.Integration.Setup.EF;
public class TestApplicationDbContext : BaseApplicationDbContext
{
    public TestApplicationDbContext(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbName = $"dbtest-{Guid.NewGuid()}";

        var connString = _configuration.GetConnectionString("TestConnection");

        connString = connString + $"database={dbName}";

        optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString));
    }
}
