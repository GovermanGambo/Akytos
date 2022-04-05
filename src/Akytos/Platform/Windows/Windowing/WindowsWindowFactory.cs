using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Akytos.Windowing;

internal class WindowsWindowFactory : IWindowFactory
{
    public IGameWindow Create(WindowProperties properties)
    {
        var windowOptions = WindowOptions.Default;
        windowOptions.Title = properties.Title;
        windowOptions.Size = new Vector2D<int>(properties.Width, properties.Height);

        var window = Window.Create(windowOptions);
        
        Input.Initialize(window.CreateInput());

        return new WindowsWindow(window);
    }
}