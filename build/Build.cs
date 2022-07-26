using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using Serilog;
using System.IO;
using System.IO.Compression;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.BundleZip);

    private const string CliProjectName = "DarkHumanizrCli";

    [Solution] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    readonly Configuration Configuration = Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution.GetProject(CliProjectName)));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution.GetProject(CliProjectName))
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Publish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(Solution.GetProject(CliProjectName))
                .SetConfiguration(Configuration)
                .SetProperty("PublishProfile", "FolderProfile")
                .SetProperty("OutDir", ArtifactsDirectory / CliProjectName)
                .SetNodeReuse(IsLocalBuild)
                .SetProperty("DeployOnBuild", "true")
                .SetProperty("Platform", "x64")
                .SetTargets("Build", "Publish"));
        });

    Target BundleZip => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            var exeVersion = BuildUtils.GetExeVersion(ArtifactsDirectory / CliProjectName / "humanizr.exe");

            var zipBundleFiles = Directory.GetFiles(ArtifactsDirectory / CliProjectName);

            var zipBundlePath = ArtifactsDirectory / $"dark-humanizr_v{exeVersion}.zip";

            CreateZipFileFromFiles(zipBundlePath, zipBundleFiles);
        });

    private void CreateZipFileFromFiles(string zipFilePath, string[] filePaths)
    {
        Log.Information("Creating a zip bundle: {ZipFilePath}", zipFilePath);

        using var zipFile = File.Open(zipFilePath, FileMode.Create);
        using var archive = new ZipArchive(zipFile, ZipArchiveMode.Create, true);

        foreach (var filePath in filePaths)
        {
            var fileName = Path.GetFileName(filePath);
            var zipArchiveEntry = archive.CreateEntryFromFile(filePath, fileName, CompressionLevel.Fastest);
            Log.Information("Added file: {ZipFile}", zipArchiveEntry);
        }

        Log.Information("Zip bundle created!");
    }
}