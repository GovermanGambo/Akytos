using Akytos.Assets;
using Akytos.Configuration;

namespace Akytos.ProjectManagement;

public class ProjectManager
{
    private readonly AppConfiguration m_appConfiguration;

    public ProjectManager(AppConfiguration appConfiguration)
    {
        m_appConfiguration = appConfiguration;
    }

    public bool LoadLastOpenedProject()
    {
        string? lastOpenedProjectDirectory = m_appConfiguration.ReadString("LastProjectDirectory");

        if (lastOpenedProjectDirectory == null)
        {
            return false;
        }

        var project = AkytosProject.Load(lastOpenedProjectDirectory);
        AkytosProject.CurrentProject = project;

        return true;
    }

    public void CreateNewProject(string projectName, string projectDirectory)
    {
        var project = new AkytosProject(projectName, projectDirectory);

        AkytosProject.CurrentProject = project;
        
        m_appConfiguration.WriteString("LastProjectDirectory", projectDirectory);
        m_appConfiguration.Save();

        Directory.CreateDirectory(Path.Combine(projectDirectory, SystemConstants.FileSystem.AssetsSubDirectory));
    }
    
}