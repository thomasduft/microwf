using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using static Bullseye.Targets;
using static SimpleExec.Command;

namespace targets;

internal static class Program
{
  private const string solution = "microwf.sln";
  private const string packOutput = "./artifacts";
  private const string nugetSource = "https://api.nuget.org/v3/index.json";
  private const string envVarMissing = " environment variable is missing. Aborting.";

  private static IList<string> packableModules = new List<string>{
    "microwf.Core",
    "microwf.Domain",
    "microwf.Infrastructure",
    "microwf.AspNetCoreEngine"
  };

  private static class Targets
  {
    public const string RestoreTools = "restore-tools";
    public const string CleanBuildOutput = "clean-build-output";
    public const string CleanPackOutput = "clean-pack-output";
    public const string Build = "build";
    public const string Test = "test";
    public const string Pack = "pack";
    public const string Deploy = "deploy";
  }

  static async Task Main(string[] args)
  {
    // TODO: encapsulate with sth. like McMaster.Extensions.CommandLineUtils
    var version = "0.0.0";
    var key = string.Empty;

    if (args[0].Contains("--"))
    {
      var firstArg = args[0].Split("--")[0].Trim();
      var customArgs = args[0].Split("--")[1].Trim().Split("&");

      if (customArgs.Any(x => x.Contains("version")))
      {
        var versionArgs = customArgs.First(x => x.Contains("version"));
        version = versionArgs.Split('=')[1].Trim();
      }

      if (customArgs.Any(x => x.Contains("key")))
      {
        var keyArgs = customArgs.First(x => x.Contains("key"));
        key = keyArgs.Split('=')[1].Trim();
      }

      args[0] = firstArg;
    }

    if (version != "0.0.0")
    {
      Console.WriteLine($"Using version: '{version}'" + Environment.NewLine);
    }

    Target(Targets.RestoreTools, () =>
    {
      Run("dotnet", "tool restore");
    });

    Target(Targets.CleanBuildOutput, () =>
    {
      Run("dotnet", $"clean {solution} -c Release -v m --nologo");
    });

    Target(Targets.Build, DependsOn(Targets.CleanBuildOutput), () =>
    {
      Run("dotnet", $"build {solution} -c Release --nologo");
    });

    Target(Targets.Test, DependsOn(Targets.Build), () =>
    {
      Run("dotnet", $"test {solution} -c Release --no-build --nologo");
    });

    Target(Targets.CleanPackOutput, () =>
    {
      if (Directory.Exists(packOutput))
      {
        Directory.Delete(packOutput, true);
      }
    });

    Target(Targets.Pack, DependsOn(Targets.Build, Targets.CleanPackOutput), () =>
    {
      if (string.IsNullOrWhiteSpace(version))
      {
        throw new Bullseye.TargetFailedException("Version for packaging is missing!");
      }

      var directory = Directory.CreateDirectory(packOutput).FullName;

      // 1. Get all possible *.csproj files
      var projects = GetFiles("src", $"*.csproj");

      // 2. iterate over them / don't allow *.Tests.csproj files
      foreach (var project in projects)
      {
        if (project.Contains(".Tests"))
          continue;

        // 3. pack them
        if (version is not null && packableModules.Any(m => project.Contains(m)))
        {
          Run("dotnet", $"pack {project} -c Release -p:PackageVersion={version} -p:Version={version} -o {directory} --no-build --nologo");
        }
      }
    });

    Target(Targets.Deploy, DependsOn(Targets.RestoreTools, Targets.Pack), () =>
    {
      if (string.IsNullOrWhiteSpace(key))
      {
        throw new Bullseye.TargetFailedException("Key for publishing is missing!");
      }

      // updating the changelog
      Console.WriteLine($"tool run releasy update-changelog -v {version} -p https://github.com/thomasduft/microwf/issues/");
      // Run("dotnet", $"tool run releasy update-changelog -v {version} -p https://github.com/thomasduft/microwf/issues/");

      // committing the changelog changes
      Console.WriteLine($"commit -am \"Committing changelog changes for v'{version}'\"");
      // Run("git", $"commit -am \"\"Committing changelog changes for v'{version}'\"");

      // applying the tag
      Console.WriteLine($"tag -a v{version} -m \"version '{version}'\"");
      // Run("git", $"tag -a v{version} -m \"version '{version}'\"");

      // push packages
      var directory = Directory.CreateDirectory(packOutput).FullName;
      var packages = GetFiles(directory, $"*.nupkg");
      foreach (var package in packages)
      {
        // 3. push them
        if (version is not null)
        {
          Console.WriteLine($"nuget push {package} -s {nugetSource} -k {key}");
          // Run("dotnet", $"nuget push {package} -s {nugetSource} -k {key}");
        }
      }
    });

    await RunTargetsAndExitAsync(
      args,
      ex => ex is SimpleExec.ExitCodeException
        || ex.Message.EndsWith(envVarMissing, StringComparison.InvariantCultureIgnoreCase)
    );
  }

  private static IEnumerable<string> GetFiles(
      string directoryToScan,
      string filter
    )
  {
    List<string> files = new List<string>();

    files.AddRange(Directory.GetFiles(
      directoryToScan,
      filter,
      SearchOption.AllDirectories
    ));

    return files;
  }
}
