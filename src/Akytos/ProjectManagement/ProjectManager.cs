using Akytos.Assets;
using Akytos.Configuration;

namespace Akytos.ProjectManagement;

internal class ProjectManager
{
    private readonly AppConfiguration m_appConfiguration;

    public ProjectManager(AppConfiguration appConfiguration)
    {
        m_appConfiguration = appConfiguration;
    }

    public AkytosProject CurrentProject { get; private set; } = null!;

    public bool LoadLastOpenedProject()
    {
        string? lastOpenedProjectDirectory = m_appConfiguration.ReadString("LastProjectDirectory");

        if (lastOpenedProjectDirectory == null)
        {
            return false;
        }

        var project = AkytosProject.Load(lastOpenedProjectDirectory);
        CurrentProject = project;
        AkytosProject.CurrentWorkingDirectory = CurrentProject.ProjectDirectory;

        return true;
    }

    public void CreateNewProject(string projectName, string projectDirectory)
    {
        var project = new AkytosProject(projectName, projectDirectory);
        
        CurrentProject = project;
        AkytosProject.CurrentWorkingDirectory = CurrentProject.ProjectDirectory;
        
        m_appConfiguration.WriteString("LastProjectDirectory", projectDirectory);
        m_appConfiguration.Save();

        Directory.CreateDirectory(Path.Combine(projectDirectory, SystemConstants.FileSystem.AssetsSubDirectory));
    }
    
}