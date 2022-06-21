using System;
using System.Collections.Generic;
using System.Linq;
using Akytos.Editor;
using Akytos.Graphics;
using Akytos.Graphics.Buffers;
using Akytos.SceneSystems;
using Akytos.Windowing;
using LightInject;
using Windmill.Actions;
using Windmill.Panels;
using Windmill.ProjectManagement;
using Windmill.Runtime;
using Windmill.Services;

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
        serviceRegistry.RegisterSingleton<AssemblyContainer>();
        serviceRegistry.RegisterSingleton<IProjectManager, ProjectManager>();
        serviceRegistry.Register<ProjectGenerator>();
        serviceRegistry.Register<AssemblyManager>();
        serviceRegistry.Register<AssemblyMonitor>();
        serviceRegistry.Register<SystemsRegistry>();
        serviceRegistry.RegisterSingleton<RuntimeManager>();

        serviceRegistry.Register<EditorHotKeyService>();

        serviceRegistry.RegisterSingleton<EditorConfiguration>();

        RegisterPanels(serviceRegistry);
        RegisterModals(serviceRegistry);
        RegisterFrameBuffers(serviceRegistry);
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
            serviceRegistry.Register(type);
        }
    }

    private static void RegisterFrameBuffers(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton<IFramebuffer>(factory =>
        {
            var editorViewport = factory.GetInstance<IEditorViewport>();
            var framebufferSpecification = new FrameBufferSpecification
            {
                Width = (uint) editorViewport.Width,
                Height = (uint) editorViewport.Height
            };
            framebufferSpecification.Attachments = new FramebufferAttachmentSpecification(
                new List<FramebufferTextureSpecification>
                {
                    new(FramebufferTextureFormat.Rgba8),
                    new(FramebufferTextureFormat.RedInteger),
                    new(FramebufferTextureFormat.Depth)
                });

            var graphicsResourceFactory = factory.GetInstance<IGraphicsResourceFactory>();
            return graphicsResourceFactory.CreateFramebuffer(framebufferSpecification);
        }, "editorFramebuffer");
        
        
        serviceRegistry.RegisterSingleton<IFramebuffer>(factory =>
        {
            var framebufferSpecification = new FrameBufferSpecification
            {
                Width = 1920,
                Height = 1080
            };
            framebufferSpecification.Attachments = new FramebufferAttachmentSpecification(
                new List<FramebufferTextureSpecification>
                {
                    new(FramebufferTextureFormat.Rgba8),
                    new(FramebufferTextureFormat.RedInteger),
                    new(FramebufferTextureFormat.Depth)
                });

            var graphicsResourceFactory = factory.GetInstance<IGraphicsResourceFactory>();
            return graphicsResourceFactory.CreateFramebuffer(framebufferSpecification);
        }, "gameFramebuffer");
    }
}