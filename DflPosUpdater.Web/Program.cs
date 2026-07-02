using DflPosUpdater.App.BLL;
using DflPosUpdater.App.BLL.Jobs;
using DflPosUpdater.App.DAL;
using DflPosUpdater.Web.Configurations;
using DflPosUpdater.Web.Middleware;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontro ConnectionStrings:DefaultConnection en appsettings.json.");

builder.Services.AddControllersWithViews();

builder.Services.Configure<FormOptions>(options =>
{
    var maxMb = builder.Configuration.GetValue<long?>("Uploads:MaxUploadSizeMB") ?? 500;
    options.MultipartBodyLengthLimit = maxMb * 1024 * 1024;
});

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddBusinessLayer();

builder.Services.AddHangfire(config =>
{
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.FromSeconds(5),
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true,
            PrepareSchemaIfNecessary = true
        });
});

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = builder.Configuration.GetValue<int?>("Hangfire:WorkerCount") ?? 5;
    options.Queues = new[] { "ftp", "default" };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

var hangfirePath = app.Configuration.GetValue<string>("Hangfire:DashboardPath") ?? "/hangfire";
app.UseHangfireDashboard(hangfirePath, new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter(app.Environment, app.Configuration) },
    DashboardTitle = "DFL POS - Jobs Hangfire"
});

RecurringJob.AddOrUpdate<RetryFailedDeploymentsJob>(
    "retry-failed-deployments",
    job => job.EjecutarAsync(),
    Cron.Hourly());

RecurringJob.AddOrUpdate<CleanupJob>(
    "cleanup-old-logs",
    job => job.EjecutarAsync(30),
    Cron.Daily());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
