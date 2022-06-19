using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Akytos;
using Akytos.Events;
using Akytos.SceneSystems;
using ImGuiNET;
using Windmill.Resources;
using Windmill.Services;

namespace Windmill.ProjectManagement;

internal class NewProjectModal : IModal
{
    private readonly IProjectManager m_projectManager;
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly EditorConfiguration m_editorConfiguration;

    private IEnumerable<string> m_errors;
    private bool m_isOpen;

    private string m_projectName = "";
    private string m_projectPath = "";
    private string m_currentProjectsRoot;

    private bool m_shouldOpen;
    private bool m_didEditPath;

    public NewProjectModal(IProjectManager projectManager, SceneEditorContext sceneEditorContext, EditorConfiguration editorConfiguration)
    {
        m_projectManager = projectManager;
        m_sceneEditorContext = sceneEditorContext;
        m_editorConfiguration = editorConfiguration;
        m_errors = Array.Empty<string>();
        
        m_errors = m_projectManager.ValidateProjectParameters(m_projectName, m_projectPath);

        m_currentProjectsRoot = m_editorConfiguration.ReadString(SystemConstants.ConfigurationKeys.CurrentProjectsRoot) ?? "";
        m_projectPath = m_currentProjectsRoot;
    }

    public void Dispose()
    {
    }

    public string Name => LocalizedStrings.NewProject;

    public bool IsOpen
    {
        get => m_isOpen;
        private set
        {
            m_isOpen = value;

            if (!value) Closing?.Invoke();
        }
    }

    public event Action? Closing;

    public void Open()
    {
        m_shouldOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    public void OnDrawGui()
    {
        if (m_shouldOpen)
        {
            ImGui.OpenPopup(Name);
            IsOpen = true;
            m_shouldOpen = false;
        }

        bool open = IsOpen;

        if (open) ImGui.SetNextWindowSize(ImGui.GetWindowSize() / 2f);

        if (!ImGui.BeginPopupModal(Name, ref open))
        {
            IsOpen = false;
            return;
        }

        if (ImGui.InputText("Project name", ref m_projectName, 50)) OnFormDataChanged(nameof(m_projectName));

        if (ImGui.InputText("Project path", ref m_projectPath, 200)) OnFormDataChanged(nameof(m_projectPath));

        foreach (string error in m_errors) ImGui.TextColored((Vector4) Color.Red, error);

        if (ImGui.Button("Create project"))
            if (!m_errors.Any())
            {
                if (!Directory.Exists(m_projectPath))
                {
                    Directory.CreateDirectory(m_projectPath);
                }
                
                m_projectManager.CreateNewProject(m_projectName, m_projectPath);
                m_sceneEditorContext.CreateNewScene<Node2D>();
                UpdateCurrentProjectsRoot();
                Close();
            }

        ImGui.EndPopup();
    }

    public void OnEvent(IEvent e)
    {
    }

    private void OnFormDataChanged(string fieldName)
    {
        if (fieldName == nameof(m_projectPath))
        {
            m_didEditPath = true;
        }

        if (!m_didEditPath)
        {
            m_projectPath = Path.Combine(m_currentProjectsRoot, m_projectName);
        }
        
        m_errors = m_projectManager.ValidateProjectParameters(m_projectName, m_projectPath, true);
    }

    private void UpdateCurrentProjectsRoot()
    {
        string path = m_projectPath[..m_projectPath.LastIndexOf(Path.DirectorySeparatorChar)];
        m_editorConfiguration.WriteString(SystemConstants.ConfigurationKeys.CurrentProjectsRoot, path);
        m_editorConfiguration.Save();
    }
}