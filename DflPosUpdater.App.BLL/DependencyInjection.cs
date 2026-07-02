using DflPosUpdater.App.BLL.Jobs;
using DflPosUpdater.App.BLL.Services.Dashboard;
using DflPosUpdater.App.BLL.Services.Deploy;
using DflPosUpdater.App.BLL.Services.FTP;
using DflPosUpdater.App.BLL.Services.Notifications;
using DflPosUpdater.App.BLL.Services.Sucursales;
using DflPosUpdater.App.BLL.Services.Updater;
using Microsoft.Extensions.DependencyInjection;

namespace DflPosUpdater.App.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISucursalService, SucursalService>();
        services.AddScoped<IVersionService, VersionService>();
        services.AddScoped<IDeployService, DeployService>();
        services.AddScoped<IFtpDeployService, FtpDeployService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUpdaterManifestService, UpdaterManifestService>();

        services.AddScoped<DeploySucursalJob>();
        services.AddScoped<RetryFailedDeploymentsJob>();
        services.AddScoped<CleanupJob>();

        return services;
    }
}
