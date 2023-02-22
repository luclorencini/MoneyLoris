using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MoneyLoris.Infrastructure.Persistence.Context;

public partial class ApplicationDbContext : BaseApplicationDbContext
{
    public ApplicationDbContext(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString));

            //descomente abaixo para ver as queries realizadas no cmd do kestrel
            //optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            //optionsBuilder.EnableSensitiveDataLogging();
        }
    }


}
