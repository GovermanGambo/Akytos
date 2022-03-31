using System.Reflection;
using Akytos.Configuration;

namespace Akytos;

internal class AkytosProject
{
    private const string ProjectExtension = SystemConstants.FileSystem.ProjectFileExtension;

    public AkytosProject(string projectName, string projectDirectory)
    {
        ProjectName = projectName;
        ProjectDirectory = projectDirectory;
        var directoryInfo = new DirectoryInfo(projectDirectory);
        LastModifiedTime = directoryInfo.LastWriteTime;
        Configuration = new ConfigurationFile(Path.Combine(projectDirectory, $"{projectName}{ProjectExtension}"));

        Configuration.WriteString("General/ProjectName", projectName);
        Configuration.WriteString("General/ProjectDirectory", projectDirectory);

        string? appVersion = Configuration.ReadString("General/AppVersion");
        
        if (appVersion == null)
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            appVersion = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
            Configuration.WriteString("General/AppVersion", appVersion);
        }

        AppVersion = new Version(appVersion);

        Configuration.Save();
    }

    public string ProjectName { get; }
    public string ProjectDirectory { get; }
    public DateTime LastModifiedTime { get; }
    public Version AppVersion { get; }

    public IConfiguration Configuration { get; }

    public static AkytosProject Load(string projectDirectory)
    {
        string? file = Directory.GetFiles(projectDirectory, $"*{ProjectExtension}").FirstOrDefault();

        if (file == null)
        {
            throw new FileNotFoundException($"Could not find any file with the {ProjectExtension} extension.");
        }

        string projectName = Path.GetFileNameWithoutExtension(file);

        var project = new AkytosProject(projectName, projectDirectory);

        return project;
    }
}