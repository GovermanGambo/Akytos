using Akytos.Configuration;

namespace Akytos.ProjectManagement;

internal class ProjectManager : IProjectManager
{
    private readonly AppConfiguration m_appConfiguration;
    private readonly ProjectGenerator m_projectGenerator;
    private AkytosProject m_currentProject;

    public ProjectManager(AppConfiguration appConfiguration, ProjectGenerator projectGenerator)
    {
        m_appConfiguration = appConfiguration;
        m_projectGenerator = projectGenerator;
    }

    public event Action? ProjectChanged;

    public AkytosProject CurrentProject
    {
        get => m_currentProject;
        private set
        {
            m_currentProject = value;
            Application.WorkingDirectory = m_currentProject.ProjectDirectory;
            ProjectChanged?.Invoke();
        }
    }

    public IEnumerable<AkytosProject> GetPreviousProjects()
    {
        var projects = m_appConfiguration.GetSection("Projects");

        return projects.Reverse().Select(s => new AkytosProject(s.Key, s.Value));
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
        
        Debug.LogInformation("Loaded project {0}", project.ProjectName);

        return true;
    }

    public void LoadProject(AkytosProject project)
    {
        string projectKey = $"Projects/{project.ProjectName}";
        m_appConfiguration.Remove(projectKey);
        m_appConfiguration.WriteString(projectKey, project.ProjectDirectory);
        m_appConfiguration.Save();

        CurrentProject = project;

        Debug.LogInformation("Loaded project {0}", project.ProjectName);
    }

    public void CreateNewProject(string projectName, string projectDirectory)
    {
        var project = m_projectGenerator.GenerateProject(projectName, projectDirectory);
        
        CurrentProject = project;
        
        string projectKey = $"Projects/{projectName}";
        m_appConfiguration.WriteString(projectKey, projectDirectory);
        m_appConfiguration.Save();
        
        Debug.LogInformation("Created project {0}", project.ProjectName);
    }
    
}