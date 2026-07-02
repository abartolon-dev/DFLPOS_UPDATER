using System.IO.Compression;

namespace DflPosUpdater.App.BLL.Helpers;

public static class ZipHelper
{
    public static bool IsValidZip(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            using var zip = ZipFile.OpenRead(path);
            return zip.Entries.Count > 0;
        }
        catch
        {
            return false;
        }
    }
}
