namespace DflPosUpdater.Web.Configurations;

public class FtpConfiguration
{
    public int ConnectTimeoutMs { get; set; } = 30000;
    public int ReadTimeoutMs { get; set; } = 30000;
    public int DataConnectionConnectTimeoutMs { get; set; } = 30000;
}
