using System.Collections;
using Akytos.Assertions;
using LightInject;

namespace Windmill.Panels;

internal class PanelManager : IEnumerable<IEditorPanel>
{
    private readonly IServiceFactory m_serviceFactory;
    private IEditorPanel[] m_panels = null!;
    private bool m_initialized;

    // TODO: Panels should be created on demand and disposed when they're closed
    public PanelManager(IServiceFactory serviceFactory)
    {
        m_serviceFactory = serviceFactory;
    }

    public void Initialize()
    {
        m_panels = m_serviceFactory.GetAllInstances<IEditorPanel>().ToArray();
        m_initialized = true;
    }

    public void OnDrawGui()
    {
        Assert.IsTrue(m_initialized, "PanelManager is not initialized!");
        
        foreach (var editorPanel in m_panels)
        {
            if (editorPanel.IsEnabled)
            {
                editorPanel.OnDrawGui();
            }
        }
    }

    public TPanel GetPanel<TPanel>() where TPanel : IEditorPanel
    {
        Assert.IsTrue(m_initialized, "PanelManager is not initialized!");
        
        var panel = m_panels.FirstOrDefault(p => p is TPanel);
        
        Assert.IsNotNull(panel, $"Panel {nameof(TPanel)} does not exist!");

        return (TPanel)panel;
    }

    public IEnumerator<IEditorPanel> GetEnumerator()
    {
        return ((IEnumerable<IEditorPanel>)m_panels).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}