using System.Text.RegularExpressions;

namespace DflPosUpdater.App.BLL.Helpers;

public static class VersionHelper
{
    private static readonly Regex VersionRegex = new("^[0-9]+(\\.[0-9]+){1,3}([.-][a-zA-Z0-9]+)?$", RegexOptions.Compiled);

    public static bool IsValidVersionText(string version)
    {
        return !string.IsNullOrWhiteSpace(version) && VersionRegex.IsMatch(version.Trim());
    }
}
