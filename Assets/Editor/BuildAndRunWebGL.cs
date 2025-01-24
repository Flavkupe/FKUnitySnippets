using UnityEngine;

using UnityEditor;
using System.Diagnostics;
using System.IO;

public class WebGLBuildScript
{
    [MenuItem("Build/Build and Run WebGL")]
    public static void BuildAndRunWebGL()
    {
        // Define the build path
        var buildPath = "build";

        // Create the build directory if it doesn't exist
        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }

        // Build the project
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, buildPath, BuildTarget.WebGL, BuildOptions.None);

        // Run the build
        RunWebGLBuild(buildPath, 8080); // You can change the port number here
    }

    public static void RunWebGLBuild(string buildPath, int port)
    {
        // Define the path to the Python script
        var pythonScriptPath = "SimpleHTTPServer.py";

        // Start the Python SimpleHTTPServer
        var startInfo = new ProcessStartInfo("python", $"{pythonScriptPath} {port}")
        {
            WorkingDirectory = buildPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = new Process
        {
            StartInfo = startInfo
        };

        process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
        process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }
}
