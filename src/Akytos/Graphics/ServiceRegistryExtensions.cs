using Akytos.Windowing;
using LightInject;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Akytos.Graphics;

public static class ServiceRegistryExtensions
{
    public static IServiceRegistry AddGraphics(this IServiceRegistry serviceRegistry, GraphicsBackend graphicsBackend)
    {
        if (graphicsBackend is not GraphicsBackend.OpenGL)
        {
            throw new NotImplementedException("Only OpenGL is supported as a graphics backend!");
        }
        
        serviceRegistry.RegisterOpenGL();
        
        serviceRegistry.RegisterSingleton<GraphicsResourceRegistry>();
        serviceRegistry.Register<ISpriteBatch, SpriteBatch>();

        return serviceRegistry;
    }

    private static IServiceRegistry RegisterOpenGL(this IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<IGraphicsResourceFactory, OpenGLGraphicsResourceFactory>();
        serviceRegistry.RegisterSingleton<GL>(factory =>
        {
            var window = factory.GetInstance<IGameWindow>().GetNativeWindow() as IWindow;
            return window.CreateOpenGL();
        });
        serviceRegistry.RegisterSingleton<IGraphicsDevice, OpenGLGraphicsDevice>();

        return serviceRegistry;
    }
}