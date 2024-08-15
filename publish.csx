#r "nuget: System.Diagnostics.Process, 4.3.0"

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.IO.Compression;

// Define output path for published applications
var output = "artifacts/Publish/";
var platforms = new List<string>(){"win-x64", "linux-x64", "osx-x64", "osx-arm64"};

void PublishTools()
{
    var name = "DxFeed.Graal.Net.Tools";
    var framework = "net8.0";
    var project = FindProject(name);

    PublishProjects(new[] { project }, name, framework, true);
}

void PublishSamples()
{
    var name = "Samples";
    var framework = "net6.0";
    var projects = FindAllProjectInDirectory("samples");

    PublishProjects(projects, name, framework, false);
}

void PublishProjects(IEnumerable<string> projects, string name, string framework, bool isSingleFile)
{
    foreach (var project in projects)
    {
        foreach (var platform in platforms)
        {
            PublishScd(project, framework, platform, $"{output}/{name}/{platform}", isSingleFile);
        }
    }

    // Sign the macOS binaries if they exist.
    Codesign($"{output}/{name}/osx-x64");
    Codesign($"{output}/{name}/osx-arm64");

    // Zip the output directory.
    ZipDirectory($"{output}/{name}", name, GetVersion());
}

// Function to publish self-contained deployment (SCD).
// Publishes the specified .NET project for the given framework and platform.
void PublishScd(string project, string framework, string platform, string output, bool isSingleFile)
{
    ExecuteCommand("dotnet", $"publish" +
                             $" {project}" +
                             $" --framework {framework}" +
                             $" -r {platform}" +
                             $" -o {output}" +
                             $" -p:PublishSingleFile={isSingleFile}" +
                             $" -p:SolutionDir={GetSolutionFolder()}" +
                             "  -p:IncludeNativeLibrariesForSelfExtract=true" +
                             "  -p:AllowedReferenceRelatedFileExtensions=none" +
                             "  -p:DebugType=None" +
                             "  -p:DebugSymbols=false" +
                             "  --self-contained true" +
                             "  -c Release");
}

bool CommandExists(string command)
{
    try
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "sh",
            Arguments = $"-c \"command -v {command}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        process.WaitForExit();
        return process.ExitCode == 0;
    }
    catch
    {
        return false;
    }
}

void ExecuteCommand(string command, string arguments)
{
    try
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
        process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"Command failed: {command} {arguments}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to execute command: {command} {arguments}. Error: {ex.Message}");
    }
}


// Function to sign macOS applications
// path: Path to the application to be signed
void Codesign(string path)
{
    // Check if codesign is available before attempting to sign
    if (CommandExists("codesign"))
    {
        foreach (var file in Directory.GetFiles(path))
        {
            ExecuteCommand("codesign", $"-f -s - {file}");
        }
    }
}

void ZipDirectory(string path, string name, string version)
{
    // Iterate through each directory in the base directory.
    foreach (var dir in Directory.GetDirectories(path))
    {
        string dirName = Path.GetFileName(dir); // Get the directory name.
        string zipFileName = $"{name}-{dirName}-{version}.zip"; // Construct the zip file name.
        string zipFilePath = Path.Combine(path, zipFileName); // Full path to the zip file.

        // Compress the directory into a zip file.
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath); // Delete the existing zip file if it exists.
        }

        ZipFile.CreateFromDirectory(dir, zipFilePath, CompressionLevel.Optimal, false);
        Console.WriteLine($"Created zip file: {zipFilePath}");
    }
}

string FindProject(string name) =>
    Directory.EnumerateFiles($"{GetSolutionFolder()}", $"{name}.csproj", SearchOption.AllDirectories).First();

List<string> FindAllProjectInDirectory(string path) =>
    Directory.EnumerateFiles($"{GetSolutionFolder()}/{path}", "*.csproj", SearchOption.AllDirectories).ToList();

string GetVersion() =>
    File.ReadLines($"{GetSolutionFolder()}/version.txt").First().Trim();

string GetSolutionFolder([CallerFilePath] string path = null) =>
    Path.GetDirectoryName(path);

PublishSamples();
PublishTools();
