using System.Collections.ObjectModel;
using Akytos.Assertions;
using ImGuiNET;
using LightInject;

namespace Windmill.Services;

public class ModalStack
{
    private readonly IServiceFactory m_serviceFactory;
    private readonly Stack<IModal> m_modals;

    public ModalStack(IServiceFactory serviceFactory)
    {
        m_serviceFactory = serviceFactory;
        m_modals = new Stack<IModal>();
    }

    public IReadOnlyCollection<IModal> Modals => new ReadOnlyCollection<IModal>(m_modals.ToArray());

    public IModal Peek()
    {
        return m_modals.Peek();
    }

    public TModal PushModal<TModal>() where TModal : IModal
    {
        var modal = m_serviceFactory.TryGetInstance<TModal>();
        
        Assert.IsNotNull(modal, $"No modal of type {typeof(TModal)} was found.");
        
        m_modals.Push(modal);

        modal.IsOpen = true;
        ImGui.OpenPopup(modal.Name);

        return modal;
    }

    public void PopModal()
    {
        var modal = m_modals.Pop();
        
        modal.Dispose();
    }

    public void Clear()
    {
        foreach (var modal in m_modals)
        {
            modal.Dispose();
        }
        
        m_modals.Clear();
    }
}