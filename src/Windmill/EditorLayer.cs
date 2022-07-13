using Akytos;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Layers;
using Akytos.SceneSystems;
using Veldrid;
using Windmill.Panels;
using Windmill.ProjectManagement;
using Windmill.Services;

namespace Windmill;

internal class EditorLayer : ILayer
{
    private readonly AssemblyManager m_assemblyManager;
    private readonly EditorHotKeyService m_editorHotKeyService;
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly MenuService m_menuService;
    private readonly ModalStack m_modalStack;
    private readonly PanelManager m_panelManager;
    private readonly IProjectManager m_projectManager;
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly FramebufferService m_framebufferService;
    private readonly SceneTree m_sceneTree;
    private readonly ICamera m_camera;
    private readonly CommandList m_commandList;

    public EditorLayer(GraphicsDevice graphicsDevice, PanelManager panelManager, MenuService menuService, ModalStack modalStack, 
        EditorHotKeyService editorHotKeyService, SceneEditorContext sceneEditorContext, IProjectManager projectManager, 
        AssemblyManager assemblyManager, SceneTree sceneTree, CommandList commandList, FramebufferService framebufferService)
    {
        m_graphicsDevice = graphicsDevice;
        m_panelManager = panelManager;
        m_menuService = menuService;
        m_modalStack = modalStack;
        m_editorHotKeyService = editorHotKeyService;
        m_sceneEditorContext = sceneEditorContext;
        m_projectManager = projectManager;
        m_assemblyManager = assemblyManager;
        m_sceneTree = sceneTree;
        m_commandList = commandList;
        m_framebufferService = framebufferService;

        m_camera = new OrthographicCamera(245, 135);
    }

    public void Dispose()
    {
    }

    public bool IsEnabled { get; set; } = true;

    public void OnAttach()
    {
        LoadProject();
        
        // TODO: This may be superfluous when the assembly gets reloaded periodically by the monitor
        m_assemblyManager.BuildAndLoadAssemblies();

        m_sceneTree.Systems.Register<SpriteRendererSystem>();
        
        m_panelManager.Show<ViewportPanel>();
        m_panelManager.Show<GamePanel>();
        m_panelManager.Show<PropertyPanel>();
        m_panelManager.Show<HierarchyPanel>();
        m_panelManager.Show<AssetsPanel>();
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        // -- UPDATE START -- //
        
        m_sceneTree.OnUpdate(time);
        
        // -- UPDATE END -- //

        m_commandList.Begin();
        m_commandList.SetFramebuffer(m_framebufferService.GetFramebuffer("Viewport"));
        m_commandList.ClearColorTarget(0, new RgbaFloat(0.1f, 0.1f, 0.1f, 1.0f));
        m_sceneTree.OnRender(m_camera);
        
        m_commandList.SetFramebuffer(m_framebufferService.GetFramebuffer("Game"));
        m_commandList.ClearColorTarget(0, new RgbaFloat(0.1f, 0.1f, 0.1f, 1.0f));
        m_sceneTree.OnRender(m_camera);

        m_commandList.End();
        m_graphicsDevice.SubmitCommands(m_commandList);
        m_graphicsDevice.SwapBuffers();
    }

    public void OnEvent(IEvent e)
    {
        m_modalStack.Peek()?.OnEvent(e);
        m_panelManager.OnEvent(e);
        m_editorHotKeyService.OnEvent(e);
    }

    public void OnDrawGui()
    {
        Dockspace.Begin();

        m_menuService.OnDrawGui();

        m_panelManager.OnDrawGui();

        m_modalStack.OnDrawGui();

        Dockspace.End();
    }

    private void LoadProject()
    {
        if (!m_projectManager.LoadLastOpenedProject())
        {
            // TODO: Display project manager window at once. May have a separate launcher program?
            m_projectManager.CreateNewProject("TestProject", "/Users/Shared/Akytos/TestProject");
            m_sceneEditorContext.CreateNewScene<Node2D>();
        }
        else
        {
            if (!m_sceneEditorContext.TryLoadPreviousScene()) m_sceneEditorContext.CreateNewScene<Node2D>();
        }
    }
}