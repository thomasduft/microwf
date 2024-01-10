using Bullseye;

using McMaster.Extensions.CommandLineUtils;

using static Bullseye.Targets;
using static SimpleExec.Command;

const string Solution = "microwf.sln";
IList<string> packableProjects = new List<string>{
  "microwf.Core",
  "microwf.Domain",
  "microwf.Infrastructure",
  "microwf.AspNetCoreEngine"
};

var app = new CommandLineApplication
{
  UsePagerForHelpText = false
};
app.HelpOption();

// 1. define custom options
var versionOption = app.Option<string>("-rv|--release-version", "Release version that should be applied to the released artifacts.", CommandOptionType.SingleValue);

// 2. translate from Bullseye to McMaster.Extensions.CommandLineUtils
app.Argument("targets", "A list of targets to run or list. If not specified, the \"default\" target will be run, or all targets will be listed.", true);
foreach (var (aliases, description) in Options.Definitions)
{
  _ = app.Option(string.Join("|", aliases), description, CommandOptionType.NoValue);
}

app.OnExecuteAsync(async _ =>
{
  // translate from McMaster.Extensions.CommandLineUtils to Bullseye
  var targets = app.Arguments[0].Values.OfType<string>();
  var options = new Options(Options.Definitions
    .Select(d => (d.Aliases[0], app.Options
      .Single(o => d.Aliases.Contains($"--{o.LongName}")).HasValue())
  ));

  #region Tooling targets
  const string RestoreTools = "restore-tools";
  const string AddChangelog = "add-changelog";

  Target(RestoreTools, () =>
  {
    Run("dotnet", "tool restore");
  });

  Target(AddChangelog, () =>
  {
    Run("dotnet", "tool run releasy add-changelog", "changelogs");
  });
  #endregion

  #region Build targets
  const string Clean = "clean";
  const string Build = "build";
  const string Test = "test";
  const string Release = "release";

  Target(Clean, () =>
  {
    Run("dotnet", $"clean {Solution} -c Release -v m --nologo");
  });

  Target(Build, DependsOn(Clean), () =>
  {
    Run("dotnet", $"build {Solution} -c Release --nologo");
  });

  Target(Test, DependsOn(Build), () =>
  {
    Run("dotnet", $"test {Solution} -c Release --no-build --nologo");
  });

  Target(Release, DependsOn(Test), () =>
  {
    if (string.IsNullOrWhiteSpace(versionOption.Value()))
    {
      throw new TargetFailedException("Version for updating changelog is missing!");
    }

    var version = versionOption.Value();
    Console.WriteLine($"Releasing version: '{version}'");

    // updating the changelog
    Run("dotnet", $"tool run releasy update-changelog -v {version} -p https://github.com/thomasduft/microwf/issues/");

    // committing the changelog changes
    Run("git", $"commit -am \"Committing changelog changes for v{version}\"");

    // applying the tag
    Run("git", $"tag -a v{version} -m \"version '{version}'\"");

    // pushing
    Run("git", $"push origin v{version}");
  });
  #endregion

  #region Deployment targets
  const string ArtifactsDirectory = "./artifacts";
  const string CleanArtifacts = "clean-artifacts";
  const string Pack = "pack";

  Target(CleanArtifacts, () =>
  {
    if (Directory.Exists(ArtifactsDirectory))
    {
      Directory.Delete(ArtifactsDirectory, true);
    }
  });

  Target(Pack, DependsOn(Build, CleanArtifacts), () =>
  {
    if (string.IsNullOrWhiteSpace(versionOption.Value()))
    {
      throw new TargetFailedException("Version for packaging is missing!");
    }

    var version = versionOption.Value();
    Console.WriteLine($"Pack for version: '{version}'");

    // pack packages
    var directory = Directory.CreateDirectory(ArtifactsDirectory).FullName;
    var projects = GetFiles("src", $"*.csproj");
    foreach (var project in projects)
    {
      if (project.Contains(".Tests"))
        continue;

      if (packableProjects.Any(m => project.Contains(m)))
      {
        Run("dotnet", $"pack {project} -c Release -p:PackageVersion={version} -p:Version={version} -o {directory} --no-build --nologo");
      }
    }
  });
  #endregion

  await RunTargetsAndExitAsync(targets, options);
});

return await app.ExecuteAsync(args);

#region Helpers
static IEnumerable<string> GetFiles(
  string directoryToScan,
  string filter
)
{
  List<string> files = new();

  files.AddRange(Directory.GetFiles(
    directoryToScan,
    filter,
    SearchOption.AllDirectories
  ));

  return files;
}

static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = false)
{
  var dir = new DirectoryInfo(sourceDir);
  if (!dir.Exists)
    throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

  foreach (FileInfo file in dir.GetFiles())
  {
    Directory.CreateDirectory(destinationDir);
    string targetFilePath = Path.Combine(destinationDir, file.Name);
    file.CopyTo(targetFilePath, true);
  }

  if (recursive)
  {
    foreach (DirectoryInfo subDir in dir.GetDirectories())
    {
      string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
      CopyDirectory(subDir.FullName, newDestinationDir, true);
    }
  }
}
#endregion
