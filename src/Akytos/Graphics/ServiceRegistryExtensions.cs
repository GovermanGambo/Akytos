using System.Reflection;
using Akytos.Windowing;
using LightInject;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Shader = Veldrid.Shader;

namespace Akytos.Graphics;

public static class ServiceRegistryExtensions
{
    public static IServiceRegistry AddGraphics(this IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterGraphicsDevice()
            .Register<ISpriteBatch, SpriteBatch>()
            .RegisterSingleton(factory =>
            {
                var graphicsDevice = factory.GetInstance<GraphicsDevice>();
                return graphicsDevice.ResourceFactory.CreateCommandList();
            })
            .RegisterSingleton<GraphicsResourceRegistry>()
            .Register<IResourceFactory, ResourceFactory>()
            .AddImGui()
            .RegisterDefaultPipeline();

        return serviceRegistry;
    }

    private static IServiceRegistry RegisterDefaultPipeline(this IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton(factory =>
        {
            var vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("a_Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                new VertexElementDescription("a_Color", VertexElementSemantic.Color, VertexElementFormat.Float4),
                new VertexElementDescription("a_UV", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("a_TextureIndex", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Int1),
                new VertexElementDescription("a_ObjectId", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Int1)
                );
            
            var graphicsDevice = factory.GetInstance<GraphicsDevice>();
            var resourceFactory = factory.GetInstance<IResourceFactory>();
            Shader[]? shaders = null;
            using (var stream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("Akytos.Resources.Shaders.Sprites_Default.glsl"))
            {
                if (stream is not null)
                {
                    shaders = resourceFactory.CreateShader(stream);
                }
            }

            if (shaders is null)
            {
                throw new ArgumentException();
            }

            var framebufferService = factory.GetInstance<FramebufferService>();
            var framebuffer = framebufferService.GetFramebuffer("Viewport");
            var output = framebuffer.OutputDescription;
            
            var pipelineDescription = new GraphicsPipelineDescription
            {
                BlendState = new BlendStateDescription(RgbaFloat.White, BlendAttachmentDescription.AlphaBlend, BlendAttachmentDescription.Disabled),
                DepthStencilState = new DepthStencilStateDescription(
                    true, true, ComparisonKind.LessEqual),
                RasterizerState = new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid,
                    FrontFace.Clockwise, true, false),
                PrimitiveTopology = PrimitiveTopology.TriangleStrip,
                ResourceLayouts = Array.Empty<ResourceLayout>(),
                ShaderSet = new ShaderSetDescription(new[] {vertexLayout}, shaders),
                Outputs = output
            };

            return graphicsDevice.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);
        }, "defaultPipeline");

        return serviceRegistry;
    }

    private static IServiceRegistry RegisterGraphicsDevice(this IServiceRegistry serviceRegistry)
    {
        var options = new GraphicsDeviceOptions
        {
            PreferDepthRangeZeroToOne = true,
            PreferStandardClipSpaceYDirection = true
        };
        
        serviceRegistry.Register<GraphicsDevice>(factory =>
        {
            var window = factory.GetInstance<IGameWindow>();
            return VeldridStartup.CreateGraphicsDevice((Sdl2Window)window.GetNativeWindow(), options);
        });

        return serviceRegistry;
    }

    private static IServiceRegistry AddImGui(this IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton(factory =>
        {
            var graphicsDevice = factory.GetInstance<GraphicsDevice>();
            var window = factory.GetInstance<IGameWindow>();
            var renderer = new ImGuiRenderer(graphicsDevice, graphicsDevice.SwapchainFramebuffer.OutputDescription, window.Width, window.Height);

            return renderer;
        });

        return serviceRegistry;
    }
}