namespace DflPosUpdater.App.BLL.Services.FTP;

public interface IFtpDeployService
{
    Task SubirVersionAsync(int despliegueId, CancellationToken cancellationToken = default);
}
