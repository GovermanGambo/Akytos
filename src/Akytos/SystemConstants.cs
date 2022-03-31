namespace Akytos;

internal static class SystemConstants
{
    public static class FileSystem
    {
        public const string AssemblySubDirectory = "Assembly";
        public const string AssetsSubDirectory = "Assets";
        public const string ProjectFileExtension = ".akproj";
        public const string SceneFileExtension = ".ascn";
        public const string AssetsDirectoryPrefix = "Assets://";
    }

    public static class CommandLine
    {
        public const string DotnetCommand = "dotnet";
        public const string ProjectTemplateShortName = "akclasslib";
        public const string CreateProjectCommand = "new {0} -n {1} -o {2}";
        public const string CreateSolutionCommand = "new sln -n {0}";
        public const string AddProjectCommand = "sln add {0}";
    }

    public static class ConfigurationKeys
    {
        public const string Projects = "Projects";
    }
}