using Akytos;

namespace Windmill.Utilities;

internal static class DotnetUtility
{
    public static void BuildSolution(string solutionDirectory, BuildConfiguration buildConfiguration)
    {
        string arguments = string.Format(SystemConstants.CommandLine.BuildCommand, buildConfiguration);
        
        RunCommand(solutionDirectory, arguments);
    }
    
    public static void CreateSolution(string solutionName, string targetDirectory)
    {
        string arguments = string.Format(SystemConstants.CommandLine.CreateSolutionCommand, solutionName);

        RunCommand(targetDirectory, arguments);  
    }

    public static void CreateProject(string projectName, string solutionDirectory)
    {
        string createArguments = string.Format(SystemConstants.CommandLine.CreateProjectCommand, SystemConstants.CommandLine.ProjectTemplateShortName, projectName, projectName);

        RunCommand(solutionDirectory, createArguments);

        string addArguments = string.Format(SystemConstants.CommandLine.AddProjectCommand, projectName);
        RunCommand(solutionDirectory, addArguments);  
    }
    
    public static void RunCommand(string workingDirectory, string arguments)
    {
        var command = new SystemCommand(SystemConstants.CommandLine.DotnetCommand);
        command.AddArgument(arguments);
        command.SetWorkingDirectory(workingDirectory);
        
        command.Run();
    }
}

public enum BuildConfiguration
{
    Debug,
    Release
}