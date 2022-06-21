using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Akytos.Events;
using LightInject;

namespace Windmill.Panels;

internal class PanelManager
{
    private readonly IServiceFactory m_serviceFactory;
    private readonly List<IEditorPanel> m_openPanels = new();
    private readonly List<PanelSummary> m_panelSummaries;

    // TODO: Panels should be created on demand and disposed when they're closed
    public PanelManager(IServiceFactory serviceFactory)
    {
        m_serviceFactory = serviceFactory;
        m_panelSummaries = m_serviceFactory.GetAllInstances<IEditorPanel>().Select(p => p.Summary).ToList();
    }

    public ReadOnlyCollection<PanelSummary> GetPanelSummaries()
    {
        return new ReadOnlyCollection<PanelSummary>(m_panelSummaries);
    }

    public bool IsPanelOpen(string id)
    {
        return m_openPanels.Any(p => p.Summary.Id == id);
    }
    
    public ReadOnlyCollection<IEditorPanel> GetPanels()
    {
        return new ReadOnlyCollection<IEditorPanel>(m_openPanels);
    }

    public void OnDrawGui()
    {
        var openPanels = new List<IEditorPanel>(m_openPanels);
        foreach (var editorPanel in openPanels)
        {
            editorPanel.OnDrawGui();
        }
    }

    public void OnRender()
    {
        var openPanels = new List<IEditorPanel>(m_openPanels);
        foreach (var editorPanel in openPanels)
        {
            editorPanel.OnRender();
        }
    }

    public void Show(PanelSummary panelSummary)
    {
        object? panel = m_serviceFactory.GetInstance(panelSummary.Type);
        if (panel is IEditorPanel editorPanel)
        {
            m_openPanels.Add(editorPanel);
            editorPanel.Closed = OnPanelClosing;
        }
    }

    private void OnPanelClosing(PanelSummary panelSummary)
    {
        Hide(panelSummary);
    }

    public void Show<TPanel>() where TPanel : IEditorPanel
    {
        var panel = m_serviceFactory.GetInstance<TPanel>();
        m_openPanels.Add(panel);
    }

    public void Hide(PanelSummary panelSummary)
    {
        var panel = m_openPanels.FirstOrDefault(p => p.Summary.Id == panelSummary.Id);
        if (panel is null)
        {
            return;
        }

        m_openPanels.Remove(panel);
        panel.Closed = null;
        panel.Dispose();
    }

    public void Hide<TPanel>() where TPanel : IEditorPanel
    {
        var panel = GetPanel<TPanel>();

        if (panel is null)
        {
            return;
        }

        m_openPanels.Remove(panel);
        panel.Dispose();
    }

    public TPanel? GetPanel<TPanel>() where TPanel : IEditorPanel
    {
        var panel = m_openPanels.OfType<TPanel>().FirstOrDefault();
        
        return panel;
    }

    public void OnEvent(IEvent e)
    {
        foreach (var editorPanel in m_openPanels)
        {
            editorPanel.OnEvent(e);
        }
    }
}