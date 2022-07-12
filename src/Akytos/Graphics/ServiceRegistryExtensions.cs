using System.Reflection;
using Akytos.Windowing;
using LightInject;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

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
            ShaderProgram? shaderProgram = null;
            using (var stream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("Akytos.Resources.Shaders.Sprites_Default.glsl"))
            {
                if (stream is not null)
                {
                    shaderProgram = resourceFactory.CreateShader(stream);
                }
            }

            if (shaderProgram is null)
            {
                throw new ArgumentException();
            }

            var pipelineDescription = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = new DepthStencilStateDescription(
                    true, true, ComparisonKind.LessEqual),
                RasterizerState = new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid,
                    FrontFace.Clockwise, true, false),
                PrimitiveTopology = PrimitiveTopology.TriangleStrip,
                ResourceLayouts = Array.Empty<ResourceLayout>(),
                ShaderSet = new ShaderSetDescription(new[] {vertexLayout}, new [] { shaderProgram.VertexShader, shaderProgram.FragmentShader}),
                Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription
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
            var window = factory.GetInstance<Sdl2Window>();
            return VeldridStartup.CreateGraphicsDevice(window, options);
        });

        return serviceRegistry;
    }
}