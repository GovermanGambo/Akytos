using System;
using System.Collections.Generic;
using System.Linq;
using Akytos;
using Akytos.Configuration;
using Akytos.Diagnostics.Logging;
using Akytos.Windowing;

namespace Windmill.ProjectManagement;

internal class ProjectManager : IProjectManager
{
    private readonly EditorConfiguration m_editorConfiguration;
    private readonly ProjectGenerator m_projectGenerator;
    private readonly IGameWindow m_gameWindow;
    private AkytosProject m_currentProject = null!;

    public ProjectManager(EditorConfiguration editorConfiguration, ProjectGenerator projectGenerator, IGameWindow gameWindow)
    {
        m_editorConfiguration = editorConfiguration;
        m_projectGenerator = projectGenerator;
        m_gameWindow = gameWindow;
    }

    public event Action? ProjectChanged;

    public AkytosProject CurrentProject
    {
        get => m_currentProject;
        private set
        {
            m_currentProject = value;
            OnProjectChanged();
        }
    }

    private void OnProjectChanged()
    {
        Application.WorkingDirectory = m_currentProject.ProjectDirectory;
        m_gameWindow.Title = $"Akytos Windmill ({m_currentProject.ProjectName})";
        ProjectChanged?.Invoke();
    }

    public IEnumerable<AkytosProject> GetPreviousProjects()
    {
        var projects = m_editorConfiguration.GetSection(SystemConstants.ConfigurationKeys.Projects);

        return projects.Reverse().Select(s => new AkytosProject(s.Key, s.Value));
    }

    public IEnumerable<string> ValidateProjectParameters(string projectName, string projectDirectory, bool ignoreNonExisting = false)
    {
        return m_projectGenerator.ValidateProjectParameters(projectName, projectDirectory, ignoreNonExisting);
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
        
        Log.Core.Information("Loaded project {0}", project.ProjectName);

        return true;
    }

    public void LoadProject(AkytosProject project)
    {
        string projectKey = $"Projects/{project.ProjectName}";
        m_editorConfiguration.Remove(projectKey);
        m_editorConfiguration.WriteString(projectKey, project.ProjectDirectory);
        m_editorConfiguration.Save();

        CurrentProject = project;

        Log.Core.Information("Loaded project {0}", project.ProjectName);
    }

    public void CreateNewProject(string projectName, string projectDirectory)
    {
        var project = m_projectGenerator.GenerateProject(projectName, projectDirectory);
        
        CurrentProject = project;
        
        string projectKey = $"Projects/{projectName}";
        m_editorConfiguration.WriteString(projectKey, projectDirectory);
        m_editorConfiguration.Save();
        
        Log.Core.Information("Created project {0}", project.ProjectName);
    }
    
}