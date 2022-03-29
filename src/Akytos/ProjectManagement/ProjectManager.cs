using Akytos.Configuration;

namespace Akytos.ProjectManagement;

internal class ProjectManager : IProjectManager
{
    private readonly AppConfiguration m_appConfiguration;
    private readonly ProjectGenerator m_projectGenerator;

    public ProjectManager(AppConfiguration appConfiguration, ProjectGenerator projectGenerator)
    {
        m_appConfiguration = appConfiguration;
        m_projectGenerator = projectGenerator;
    }

    public AkytosProject CurrentProject { get; private set; }

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
        
        if (!Directory.Exists(projectDirectory))
        {
            errors.Add("Project directory does not exist!");
        }

        if (Directory.EnumerateFileSystemEntries(projectDirectory).Any())
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
        var project = m_projectGenerator.GenerateProject(projectName, projectDirectory);
        
        CurrentProject = project;
        AkytosProject.CurrentWorkingDirectory = CurrentProject.ProjectDirectory;
        
        string projectKey = $"Projects/{projectName}";
        m_appConfiguration.WriteString(projectKey, projectDirectory);
        m_appConfiguration.Save();  

        Directory.CreateDirectory(Path.Combine(projectDirectory, SystemConstants.FileSystem.AssetsSubDirectory));
    }
    
}