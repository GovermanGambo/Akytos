using Akytos.Windowing;
using LightInject;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Akytos.Graphics;

internal class GraphicsCompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton<GL>(factory =>
        {
            var window = factory.GetInstance<IGameWindow>().GetNativeWindow() as IWindow;
            return window.CreateOpenGL();
        });
        serviceRegistry.RegisterSingleton<IGraphicsDevice, OpenGLGraphicsDevice>();
        serviceRegistry.RegisterSingleton<GraphicsResourceRegistry>();

        serviceRegistry.Register<IGraphicsResourceFactory, OpenGLGraphicsResourceFactory>();
    }
}