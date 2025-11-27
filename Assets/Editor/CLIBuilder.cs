using System;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.Build.Reporting;

public static class CommandLineBuild
{
    // Opens the project, generating project files.
    public static void GenerateSLN() {}

    public static void BuildGame()
    {
        // OpenCSharpProject();
        string[] args = Environment.GetCommandLineArgs();

        string buildTargetArg = GetArg(args, "-buildTarget") ?? GetArg(args, "-buildtarget") ?? GetArg(args, "-build-target");
        string distDir = GetArg(args, "-distDir") ?? GetArg(args, "-dist-dir") ?? GetArg(args, "-dist_dir") ?? "dist";

        BuildTarget target = ParseBuildTarget(buildTargetArg) ?? BuildTarget.StandaloneWindows64;

        // Collect enabled scenes from EditorBuildSettings
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            UnityEngine.Debug.LogError("No enabled scenes found in Build Settings. Aborting build.");
            EditorApplication.Exit(1);
            return;
        }

        // Ensure output directory exists
        try
        {
            Directory.CreateDirectory(distDir);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Failed to create dist directory '{distDir}': {ex}");
            EditorApplication.Exit(1);
            return;
        }

        // Decide output path based on target
        string outputPath;
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                outputPath = Path.Combine(distDir, "The-Herd.exe");
                break;
            case BuildTarget.StandaloneOSX:
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
                outputPath = Path.Combine(distDir, "The-Herd.app");
                break;
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinuxUniversal:
                outputPath = Path.Combine(distDir, "The-Herd.x86_64");
                break;
            default:
                // For other targets, write to a folder inside dist
                outputPath = distDir;
                break;
        }

        UnityEngine.Debug.Log($"Building target: {target} -> {outputPath}");

        BuildPlayerOptions opts = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(opts);

        if (report == null)
        {
            UnityEngine.Debug.LogError("BuildPipeline returned null report.");
            EditorApplication.Exit(1);
            return;
        }

        if (report.summary.result != BuildResult.Succeeded)
        {
            UnityEngine.Debug.LogError($"Build failed: {report.summary.result} - {report.summary.totalErrors} errors");
            EditorApplication.Exit(1);  // error exit code
            return;
        }

        UnityEngine.Debug.Log("Build succeeded.");
        EditorApplication.Exit(0);      // success
    }

    static string GetArg(string[] args, string name)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 < args.Length)
                    return args[i + 1];
                return null;
            }

            // support -flag=value
            if (args[i].StartsWith(name + "=", StringComparison.OrdinalIgnoreCase))
            {
                return args[i].Substring(name.Length + 1);
            }
        }

        return null;
    }

    static BuildTarget? ParseBuildTarget(string val)
    {
        if (string.IsNullOrEmpty(val))
            return null;

        switch (val.ToLowerInvariant())
        {
            case "standalonewindows64":
            case "windows64":
            case "win64":
            case "standalonewindows":
                return BuildTarget.StandaloneWindows64;
            case "standalonewindows32":
            case "win32":
                return BuildTarget.StandaloneWindows;
            case "standaloneosx":
            case "osx":
            case "macos":
                return BuildTarget.StandaloneOSX;
            case "standalonelinux64":
            case "linux64":
                return BuildTarget.StandaloneLinux64;
            case "android":
                return BuildTarget.Android;
            case "ios":
                return BuildTarget.iOS;
            case "webgl":
                return BuildTarget.WebGL;
            default:
                // try parse as enum name
                try
                {
                    return (BuildTarget)Enum.Parse(typeof(BuildTarget), val, true);
                }
                catch
                {
                    UnityEngine.Debug.LogWarning($"Unknown build target '{val}', defaulting to StandaloneWindows64.");
                    return BuildTarget.StandaloneWindows64;
                }
        }
    }
}