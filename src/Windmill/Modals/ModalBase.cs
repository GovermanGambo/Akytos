using System;
using Akytos.Events;
using ImGuiNET;
using Windmill.Resources;

namespace Windmill.Modals;

internal abstract class ModalBase : IModal
{
    private bool m_shouldOpen;
    private bool m_isOpen;

    public void Dispose()
    {
        
    }

    public string Name { get; }

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

        ImGui.EndPopup();
    }

    public void OnEvent(IEvent e)
    {
        throw new NotImplementedException();
    }
}