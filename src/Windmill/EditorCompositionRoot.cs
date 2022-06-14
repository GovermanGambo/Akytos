using System;
using System.Linq;
using Akytos;
using Akytos.Editor;
using Akytos.SceneSystems;
using Akytos.Windowing;
using LightInject;
using Serilog;
using Serilog.Events;
using Windmill.Actions;
using Windmill.Diagnostics.Logging;
using Windmill.Panels;
using Windmill.ProjectManagement;
using Windmill.Services;
using Log = Akytos.Diagnostics.Logging.Log;

namespace Windmill;

public class EditorCompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton<EditorLayer>();
        serviceRegistry.RegisterSingleton<IEditorViewport>(factory =>
        {
            var window = factory.GetInstance<IGameWindow>();
            return new EditorViewport(window.Width, window.Height);
        });

        // TODO: Ensure that this override actually works
        serviceRegistry.RegisterSingleton(factory => new SceneTree(factory.GetInstance<SystemsRegistry>(), SceneProcessMode.Editor));
        serviceRegistry.RegisterSingleton<PanelManager>();
        serviceRegistry.RegisterSingleton<SceneEditorContext>();
        serviceRegistry.Register<MenuService>();
        serviceRegistry.Register<GizmoService>();
        serviceRegistry.RegisterSingleton<ModalStack>();
        serviceRegistry.RegisterSingleton<ActionExecutor>();
        serviceRegistry.RegisterSingleton<IProjectManager, ProjectManager>();
        serviceRegistry.Register<ProjectGenerator>();
        serviceRegistry.Register<AssemblyManager>();
        serviceRegistry.RegisterSingleton<AssemblyContainer>();
        serviceRegistry.Register<AssemblyMonitor>();
        serviceRegistry.Register<SystemsRegistry>();

        serviceRegistry.Register<EditorHotKeyService>();

        serviceRegistry.RegisterSingleton<EditorConfiguration>();

        RegisterPanels(serviceRegistry);
        RegisterModals(serviceRegistry);
    }

    private static void RegisterModals(IServiceRegistry serviceRegistry)
    {
        var modalType = typeof(IModal);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => modalType.IsAssignableFrom(p) && !p.IsInterface);

        foreach (var type in types)
        {
            serviceRegistry.RegisterSingleton(type);
        }
    }

    private static void RegisterPanels(IServiceRegistry serviceRegistry)
    {
        var panelType = typeof(IEditorPanel);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => panelType.IsAssignableFrom(p) && !p.IsInterface);

        foreach (var type in types)
        {
            serviceRegistry.RegisterSingleton(panelType, type, type.Name);
        }
    }
}