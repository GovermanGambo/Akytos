using System;
using System.Linq;
using Akytos.Editor;
using Akytos.Graphics;
using Akytos.SceneSystems;
using Akytos.Windowing;
using LightInject;
using Veldrid;
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
        var panelType = typeof(EditorPanel);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => panelType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

        foreach (var type in types)
        {
            serviceRegistry.Register(type);
        }
    }

    private static void RegisterFrameBuffers(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton(factory =>
        {
            var editorViewport = factory.GetInstance<IEditorViewport>();
            var resourceFactory = factory.GetInstance<IResourceFactory>();
            var resourceRegistry = factory.GetInstance<GraphicsResourceRegistry>();

            var rgbaAttachment = resourceFactory.CreateTexture(editorViewport.Width, editorViewport.Height,
                PixelFormat.R8_G8_B8_A8_UInt, TextureUsage.RenderTarget);
            var redIntegerAttachment = resourceFactory.CreateTexture(editorViewport.Width, editorViewport.Height,
                PixelFormat.R32_UInt, TextureUsage.RenderTarget);
            var depthAttachment = resourceFactory.CreateTexture(editorViewport.Width, editorViewport.Height,
                PixelFormat.D32_Float_S8_UInt, TextureUsage.DepthStencil);

            var framebufferDescription = new FramebufferDescription(
                new FramebufferAttachmentDescription(depthAttachment, 0),
                new[]
                {
                    new FramebufferAttachmentDescription(rgbaAttachment, 0),
                    new FramebufferAttachmentDescription(redIntegerAttachment, 0)
                });

            int width = 1920;
            int height = 1080;

            var gameRgbaAttachment = resourceFactory.CreateTexture(width, height,
                PixelFormat.R8_G8_B8_A8_UInt, TextureUsage.RenderTarget);
            var gameRedIntegerAttachment = resourceFactory.CreateTexture(width, height,
                PixelFormat.R32_UInt, TextureUsage.RenderTarget);
            var gameDepthAttachment = resourceFactory.CreateTexture(width, height,
                PixelFormat.D32_Float_S8_UInt, TextureUsage.DepthStencil);

            var gameFramebufferDescription = new FramebufferDescription(
                new FramebufferAttachmentDescription(gameDepthAttachment, 0),
                new[]
                {
                    new FramebufferAttachmentDescription(gameRgbaAttachment, 0),
                    new FramebufferAttachmentDescription(gameRedIntegerAttachment, 0)
                });

            var framebufferService = new FramebufferService(resourceFactory, resourceRegistry);
            framebufferService.CreateFramebuffer("Viewport", framebufferDescription);
            framebufferService.CreateFramebuffer("Game", gameFramebufferDescription);
            return framebufferService;
        });
    }
}