using System.Text.RegularExpressions;

namespace DflPosUpdater.App.BLL.Helpers;

public static class FileHelper
{
    private static readonly Regex InvalidCharsRegex = new("[^a-zA-Z0-9._-]", RegexOptions.Compiled);

    public static string ToSafeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "file";
        }

        var fileName = Path.GetFileName(value.Trim());
        return InvalidCharsRegex.Replace(fileName, "_");
    }

    public static string ToSafeFolderName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "folder";
        }

        return InvalidCharsRegex.Replace(value.Trim(), "_");
    }

    public static string ToWebPath(params string[] parts)
    {
        return "/" + string.Join('/', parts.Select(p => p.Trim().Trim('/', '\\')).Where(p => !string.IsNullOrWhiteSpace(p)));
    }

    public static string ToPhysicalPath(string webRootPath, string webPath)
    {
        var relative = webPath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(webRootPath, relative);
    }

    public static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
