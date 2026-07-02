using DflPosUpdater.App.BLL.Services.FTP;
using Hangfire;

namespace DflPosUpdater.App.BLL.Jobs;

public class DeploySucursalJob
{
    private readonly IFtpDeployService _ftpDeployService;

    public DeploySucursalJob(IFtpDeployService ftpDeployService)
    {
        _ftpDeployService = ftpDeployService;
    }

    [Queue("ftp")]
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task EjecutarAsync(int despliegueId)
    {
        await _ftpDeployService.SubirVersionAsync(despliegueId);
    }
}
