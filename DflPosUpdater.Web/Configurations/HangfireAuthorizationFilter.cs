using Hangfire.Dashboard;
using System.Net;

namespace DflPosUpdater.Web.Configurations;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public HangfireAuthorizationFilter(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // 1) En desarrollo, si la config lo permite, dejamos pasar sin más.
        if (_environment.IsDevelopment() &&
            _configuration.GetValue<bool>("Hangfire:AllowDashboardWithoutAuthInDevelopment"))
        {
            return true;
        }

        // 2) Si el usuario está autenticado y es Admin, permitir.
        if (httpContext.User.Identity?.IsAuthenticated == true &&
             httpContext.User.IsInRole("Admin"))
        {
            return true;
        }

        // 3) Fallback: permitir solo si la petición viene del propio servidor.
        //    Útil cuando no hay Identity configurado todavía y accedes desde la misma máquina.
        var remoteIp = httpContext.Connection.RemoteIpAddress;
        if (remoteIp != null &&
            (IPAddress.IsLoopback(remoteIp) ||
             remoteIp.Equals(httpContext.Connection.LocalIpAddress)))
        {
            return true;
        }

        return false;
    }
}
