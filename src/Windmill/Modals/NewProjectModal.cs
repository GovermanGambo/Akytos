using System;
using Akytos.Events;
using Akytos.ProjectManagement;
using ImGuiNET;
using Windmill.Resources;

namespace Windmill.Modals;

internal class NewProjectModal : IModal
{
    private readonly IProjectManager m_projectManager;
    
    private bool m_shouldOpen;
    private bool m_isOpen;

    private string m_projectName = "";
    private string m_projectPath = "";

    public NewProjectModal(IProjectManager projectManager)
    {
        m_projectManager = projectManager;
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

        if (ImGui.InputText("Project name", ref m_projectName, 50))
        {
            OnProjectNameChanged();
        }
        
        if (ImGui.InputText("Project path", ref m_projectPath, 200))
        {
            OnProjectPathChanged();
        }

        ImGui.EndPopup();
    }

    private void OnProjectPathChanged()
    {
        
    }

    private void OnProjectNameChanged()
    {
        
    }

    public void OnEvent(IEvent e)
    {
    }
}