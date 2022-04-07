using System;
using System.Linq;
using Akytos;
using Akytos.Editor;
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
        serviceRegistry.RegisterSingleton(_ => new SceneTree(SceneProcessMode.Editor));
        serviceRegistry.RegisterSingleton<PanelManager>();
        serviceRegistry.RegisterSingleton<SceneEditorContext>();
        serviceRegistry.Register<MenuService>();
        serviceRegistry.Register<GizmoService>();
        serviceRegistry.RegisterSingleton<ModalStack>();
        serviceRegistry.RegisterSingleton<ActionExecutor>();
        serviceRegistry.RegisterSingleton<IProjectManager, ProjectManager>();
        serviceRegistry.Register<ProjectGenerator>();
        serviceRegistry.Register<AssemblyManager>();
        serviceRegistry.Register<SystemsRegistry>();
        
        serviceRegistry.Register<EditorHotKeyService>();

        serviceRegistry.RegisterSingleton<EditorConfiguration>();

        RegisterPanels(serviceRegistry);
        RegisterModals(serviceRegistry);
        
        ConfigureLogging(serviceRegistry);
    }

    private static void ConfigureLogging(IServiceRegistry serviceRegistry)
    {
        var consoleService = new ConsoleService();
        Log.InitializeLogging((coreConfiguration, clientConfiguration) =>
        {
            coreConfiguration
#if DEBUG_EDITOR || DEBUG
                .WriteTo.Console()
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .WriteTo.File(SystemConstants.FileSystem.LogOutputFile, rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .WriteTo.ConsoleService(consoleService, LogEventLevel.Error);

            clientConfiguration
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.ConsoleService(consoleService);
        });

        serviceRegistry.RegisterSingleton(_ => consoleService);
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