using System.Diagnostics;

namespace Akytos.ProjectManagement;

internal class ProjectGenerator
{
    public AkytosProject GenerateProject(string projectName, string projectDirectory)
    {
        var errors = ValidateProjectParameters(projectName, projectDirectory).ToList();
        if (errors.Any())
        {
            throw new ArgumentException($"Could not generate new project. Error: {errors.FirstOrDefault()}");
        }
        
        CreateFolders(projectDirectory);
        
        CreateSolution(projectName, projectDirectory);
        
        CreateProject(projectName, projectDirectory);
        
        AddProject(projectName, projectDirectory);

        return new AkytosProject(projectName, projectDirectory);
    }
    
    public IEnumerable<string> ValidateProjectParameters(string projectName, string projectDirectory)
    {
        var errors = new List<string>();
        if (projectName == string.Empty)
        {
            // TODO: Localize errors
            errors.Add("Project name cannot be empty!");
        }

        if (projectDirectory == string.Empty)
        {
            errors.Add("Project directory cannot be empty!");
        }
        else if (!Directory.Exists(projectDirectory))
        {
            errors.Add("Project directory does not exist!");
        }
        else if (Directory.EnumerateFileSystemEntries(projectDirectory).Any())
        {
            errors.Add("Project directory must be empty!");
        }

        return errors;
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