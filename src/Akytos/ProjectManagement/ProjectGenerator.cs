using System.Diagnostics;
using Akytos.Utilities;

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

        string assemblyDirectory = Path.Combine(projectDirectory, SystemConstants.FileSystem.AssemblySubDirectory);
        
        CreateSolution(projectName, assemblyDirectory);
        
        CreateProject(projectName, assemblyDirectory);
        
        AddProject(projectName, assemblyDirectory);

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

    private static void AddProject(string projectName, string workingDirectory)
    {
        string arguments = string.Format(SystemConstants.CommandLine.AddProjectCommand, projectName);

        RunDotnetCommand(arguments, workingDirectory);  
    }

    private static void CreateSolution(string projectName, string workingDirectory)
    {
        string arguments = string.Format(SystemConstants.CommandLine.CreateSolutionCommand, projectName);

        RunDotnetCommand(arguments, workingDirectory);  
    }

    private static void CreateProject(string projectName, string workingDirectory)
    {
        string arguments = string.Format(SystemConstants.CommandLine.CreateProjectCommand, SystemConstants.CommandLine.ProjectTemplateShortName, projectName);

        RunDotnetCommand(arguments, workingDirectory);   
    }

    private void CreateFolders(string projectDirectory)
    {
        Directory.CreateDirectory(Path.Combine(projectDirectory, SystemConstants.FileSystem.AssemblySubDirectory));
        Directory.CreateDirectory(Path.Combine(projectDirectory, SystemConstants.FileSystem.AssetsSubDirectory));
    }

    private static void RunDotnetCommand(string workingDirectory, string arguments)
    {
        var command = new SystemCommand(SystemConstants.CommandLine.DotnetCommand);
        command.AddArgument(arguments);
        command.SetWorkingDirectory(workingDirectory);
        
        command.Run();
    }
}