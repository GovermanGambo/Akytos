using Veldrid.StartupUtilities;

namespace Akytos.Windowing;

public class WindowFactory : IWindowFactory
{
    public IGameWindow Create(WindowProperties properties)
    {
        var createInfo = new WindowCreateInfo
        {
            WindowWidth = properties.Width,
            WindowHeight = properties.Height,
            WindowTitle = properties.Title
        };

        var internalWindow = VeldridStartup.CreateWindow(createInfo);
        var window = new GameWindow(internalWindow);
        return window;
    }
}