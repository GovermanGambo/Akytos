using Akytos.Configuration;

namespace Akytos.ProjectManagement;

internal class ProjectManager : IProjectManager
{
    private readonly AppConfiguration m_appConfiguration;

    public ProjectManager(AppConfiguration appConfiguration)
    {
        m_appConfiguration = appConfiguration;
    }

    public AkytosProject CurrentProject { get; private set; }

    public IEnumerable<AkytosProject> GetPreviousProjects()
    {
        var projects = m_appConfiguration.GetSection("Projects");

        return projects.Reverse().Select(s => new AkytosProject(s.Key, s.Value));
    }

    public bool LoadLastOpenedProject()
    {
        var previousProjects = GetPreviousProjects();
        string? lastOpenedProjectDirectory = previousProjects.FirstOrDefault()?.ProjectDirectory;

        if (lastOpenedProjectDirectory == null)
        {
            return false;
        }
        
        var project = AkytosProject.Load(lastOpenedProjectDirectory);
        CurrentProject = project;
        AkytosProject.CurrentWorkingDirectory = CurrentProject.ProjectDirectory;

        return true;
    }

    public void LoadProject(AkytosProject project)
    {
        string projectKey = $"Projects/{project.ProjectName}";
        m_appConfiguration.Remove(projectKey);
        m_appConfiguration.WriteString(projectKey, project.ProjectDirectory);
        m_appConfiguration.Save();
        
        Application.Restart();
    }

    public void CreateNewProject(string projectName, string projectDirectory)
    {
        var project = new AkytosProject(projectName, projectDirectory);
        
        CurrentProject = project;
        AkytosProject.CurrentWorkingDirectory = CurrentProject.ProjectDirectory;
        
        string projectKey = $"Projects/{projectName}";
        m_appConfiguration.WriteString(projectKey, projectDirectory);
        m_appConfiguration.Save();

        Directory.CreateDirectory(Path.Combine(projectDirectory, SystemConstants.FileSystem.AssetsSubDirectory));
    }
    
}