using System.Diagnostics;

namespace Akytos.ProjectManagement;

internal class ProjectGenerator
{
    public AkytosProject GenerateProject(string projectName, string projectDirectory)
    {
        CreateFolders(projectDirectory);
        
        CreateSolution(projectName, projectDirectory);
        
        CreateProject(projectName, projectDirectory);
        
        AddProject(projectName, projectDirectory);

        return new AkytosProject(projectName, projectDirectory);
    }

    private void AddProject(string projectName, string projectDirectory)
    {
        string command = string.Format(SystemConstants.CommandLine.AddProjectCommand, projectName);
        var startInfo = new ProcessStartInfo(SystemConstants.CommandLine.DotnetCommand)
        {
            Arguments = command,
            WorkingDirectory = Path.Combine(projectDirectory, SystemConstants.FileSystem.AssemblySubDirectory)
        };
        var process = Process.Start(startInfo);

        if (process == null)
        {
            throw new Exception();
        }
        
        process.WaitForExit();
    }

    private void CreateSolution(string projectName, string projectDirectory)
    {
        string command = string.Format(SystemConstants.CommandLine.CreateSolutionCommand, projectName);
        var startInfo = new ProcessStartInfo(SystemConstants.CommandLine.DotnetCommand);
        startInfo.Arguments = command;
        startInfo.WorkingDirectory = Path.Combine(projectDirectory, SystemConstants.FileSystem.AssemblySubDirectory);
        var process = Process.Start(startInfo);

        if (process == null)
        {
            throw new Exception();
        }
        
        process.WaitForExit();
    }

    private void CreateProject(string projectName, string projectDirectory)
    {
        string command = string.Format(SystemConstants.CommandLine.CreateProjectCommand, SystemConstants.CommandLine.ProjectTemplateShortName, projectName);
        var startInfo = new ProcessStartInfo(SystemConstants.CommandLine.DotnetCommand);
        startInfo.Arguments = command;
        startInfo.WorkingDirectory = Path.Combine(projectDirectory, SystemConstants.FileSystem.AssemblySubDirectory);
        var process = Process.Start(startInfo);

        if (process == null)
        {
            throw new Exception();
        }
        
        process.WaitForExit();
    }

    private void CreateFolders(string workingDirectory)
    {
        Directory.CreateDirectory(Path.Combine(workingDirectory, SystemConstants.FileSystem.AssemblySubDirectory));
        Directory.CreateDirectory(Path.Combine(workingDirectory, SystemConstants.FileSystem.AssetsSubDirectory));
    }
}