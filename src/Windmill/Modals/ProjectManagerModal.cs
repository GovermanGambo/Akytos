using System;
using System.Numerics;
using Akytos.Events;
using Akytos.ProjectManagement;
using ImGuiNET;
using Windmill.Resources;

namespace Windmill.Modals;

internal class ProjectManagerModal : IModal
{
    private readonly IProjectManager m_projectManager;
    
    private bool m_isOpen;
    private bool m_shouldOpen;
    private AkytosProject? m_selectedProject;

    public ProjectManagerModal(IProjectManager projectManager)
    {
        m_projectManager = projectManager;
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
                if (ImGui.Selectable(project.ProjectName))
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
    }

    public void OnEvent(IEvent e)
    {
    }
}