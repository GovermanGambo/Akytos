using System;
using System.Numerics;
using Akytos;
using Akytos.Events;
using Akytos.SceneSystems;
using ImGuiNET;
using Windmill.Resources;
using Windmill.Services;

namespace Windmill.ProjectManagement;

internal class ProjectManagerModal : IModal
{
    private readonly IProjectManager m_projectManager;
    
    private bool m_isOpen;
    private bool m_shouldOpen;
    private AkytosProject? m_selectedProject;
    private readonly ModalStack m_modalStack;
    private readonly SceneEditorContext m_sceneEditorContext;

    public ProjectManagerModal(IProjectManager projectManager, ModalStack modalStack, SceneEditorContext sceneEditorContext)
    {
        m_projectManager = projectManager;
        m_modalStack = modalStack;
        m_sceneEditorContext = sceneEditorContext;
    }

    public void Dispose()
    {
        
    }

    public string Name => LocalizedStrings.ProjectManager;

    public bool IsOpen
    {
        get => m_isOpen;
        private set
        {
            m_isOpen = value;
            
            if (!value)
            {
                Closing?.Invoke();
            }
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

        if (open)
        {
            ImGui.SetNextWindowSize(ImGui.GetWindowSize() / 2f);
        }
        
        if (!ImGui.BeginPopupModal(Name, ref open))
        {
            IsOpen = false;
            return;
        }

        if (ImGui.Button("New project"))
        {
            Close();
            m_modalStack.PushModal<NewProjectModal>();
        }
        
        ImGui.Text(LocalizedStrings.PreviousProjects);
        
        DrawProjectsList();
        
        ImGui.EndPopup();
    }

    private void DrawProjectsList()
    {
        float height = ImGui.GetFrameHeight() - ImGui.GetTextLineHeightWithSpacing() * 2f - 10f;

        if (ImGui.BeginChildFrame(ImGui.GetID("frame"),
                new Vector2(ImGui.GetWindowWidth(), height)))
        {
            var projects = m_projectManager.GetPreviousProjects();

            foreach (var project in projects)
            {
                if (ImGui.Selectable(project.ProjectName, m_selectedProject == project, ImGuiSelectableFlags.SpanAllColumns))
                {
                    m_selectedProject = project;
                }

                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(0))
                {
                    OpenProject(project);
                }
            }
            
            ImGui.EndChildFrame();
        }
    }

    private void OpenProject(AkytosProject akytosProject)
    {
        m_projectManager.LoadProject(akytosProject);
        if (!m_sceneEditorContext.TryLoadPreviousScene())
        {
            m_sceneEditorContext.CreateNewScene<Node2D>();
        }
        
        Close();
    }

    public void OnEvent(IEvent e)
    {
    }
}