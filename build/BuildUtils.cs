using System.Diagnostics;

public static class BuildUtils
{
    public static string GetExeVersion(string pathToExe)
    {
        var versionInfo = FileVersionInfo.GetVersionInfo(pathToExe);
        return versionInfo.ProductVersion;
    }
}

