using Hangfire.Dashboard;

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

        if (_environment.IsDevelopment() &&
            _configuration.GetValue<bool>("Hangfire:AllowDashboardWithoutAuthInDevelopment"))
        {
            return true;
        }

        // Cuando agregues ASP.NET Identity, cambia esta validación por rol Admin.
        return httpContext.User.Identity?.IsAuthenticated == true &&
               httpContext.User.IsInRole("Admin");
    }
}
