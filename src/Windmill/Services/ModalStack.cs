using System.Collections.Generic;
using System.Collections.ObjectModel;
using Akytos.Diagnostics;
using ImGuiNET;
using LightInject;

namespace Windmill.Services;

internal class ModalStack
{
    private readonly IServiceFactory m_serviceFactory;
    private readonly Stack<IModal> m_modals;

    private List<IModal> m_modalQueue;
    private bool m_shouldPop;

    public ModalStack(IServiceFactory serviceFactory)
    {
        m_serviceFactory = serviceFactory;
        m_modals = new Stack<IModal>();
        m_modalQueue = new List<IModal>();
    }

    public IReadOnlyCollection<IModal> Modals => new ReadOnlyCollection<IModal>(m_modals.ToArray());

    public IModal? Peek()
    {
        return m_modals.Count > 0 ? m_modals.Peek() : null;
    }

    public void OnDrawGui()
    {
        foreach (var modal in m_modals)
        {
            modal.OnDrawGui();
        }

        if (m_shouldPop)
        {
            PopModal();
            m_shouldPop = false;
        }

        foreach (var modal in m_modalQueue)
        {
            modal.Open();
            m_modals.Push(modal);
        }
        m_modalQueue.Clear();
    }

    public TModal PushModal<TModal>() where TModal : IModal
    {
        var modal = m_serviceFactory.TryGetInstance<TModal>();
        
        Assert.IsNotNull(modal, $"No modal of type {typeof(TModal)} was found.");

        modal.Closing += ModalOnClosing;
        
        m_modalQueue.Add(modal);

        return modal;
    }

    private void ModalOnClosing()
    {
        m_shouldPop = true;
    }

    private void PopModal()
    {
        var modal = m_modals.Pop();

        if (modal.IsOpen)
        {
            modal.Close();
        }

        modal.Closing -= ModalOnClosing;

        modal.Dispose();
    }

    public void Clear()
    {
        foreach (var modal in m_modals)
        {
            modal.Closing -= ModalOnClosing;
            modal.Dispose();
        }
        
        m_modals.Clear();
    }
}