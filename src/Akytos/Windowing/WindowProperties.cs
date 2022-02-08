namespace Akytos.Windowing;

public struct WindowProperties
{
    public WindowProperties(string title, int width, int height)
    {
        Title = title;
        Width = width;
        Height = height;
    }
    
    public string Title { get; }
    public int Width { get; }
    public int Height { get; }
}