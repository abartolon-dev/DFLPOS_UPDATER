using DflPosUpdater.App.DAL.DFLSAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DflPosUpdater.App.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontro ConnectionStrings:DefaultConnection en appsettings.json.");

        var dflSaiConnectionString = configuration.GetConnectionString("DFLSAIEntities")
            ?? throw new InvalidOperationException("No se encontro ConnectionStrings:DFLSAIEntities en appsettings.json.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
        });

        services.AddDbContext<DflSaiDbContext>(options =>
        {
            options.UseSqlServer(dflSaiConnectionString, sql => sql.EnableRetryOnFailure());
        });

        return services;
    }
}
