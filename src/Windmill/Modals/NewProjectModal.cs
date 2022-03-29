using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Akytos;
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

    private IEnumerable<string> m_errors;

    public NewProjectModal(IProjectManager projectManager)
    {
        m_projectManager = projectManager;
        m_errors = Array.Empty<string>();
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
            OnFormDataChanged();
        }
        
        if (ImGui.InputText("Project path", ref m_projectPath, 200))
        {
            OnFormDataChanged();
        }
        
        foreach (string error in m_errors)
        {
            ImGui.TextColored((Vector4)Color.Red, error);
        }

        if (ImGui.Button("Create project"))
        {
            if (!m_errors.Any())
            {
                m_projectManager.CreateNewProject(m_projectName, m_projectPath);
                Close();
            }
        }

        ImGui.EndPopup();
    }

    private void OnFormDataChanged()
    {
        m_errors = m_projectManager.ValidateProjectParameters(m_projectName, m_projectPath);
    }

    public void OnEvent(IEvent e)
    {
    }
}