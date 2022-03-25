using Akytos.Configuration;

namespace Akytos.ProjectManagement;

internal class AkytosProject
{
    private const string ProjectExtension = ".akproj";

    public AkytosProject(string projectName, string projectDirectory)
    {
        ProjectName = projectName;
        ProjectDirectory = projectDirectory;
        Configuration = new AppConfiguration(Path.Combine(projectDirectory, $"{projectName}{ProjectExtension}"));

        Configuration.WriteString("ProjectName", projectName);
        Configuration.WriteString("ProjectDirectory", projectDirectory);

        Configuration.Save();
    }

    public string ProjectName { get; }
    public string ProjectDirectory { get; }

    public AppConfiguration Configuration { get; }
    
    public static string CurrentWorkingDirectory { get; set; }

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