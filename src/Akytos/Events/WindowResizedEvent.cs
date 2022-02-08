namespace Akytos.Events;

internal class WindowResizedEvent : IEvent
{
    public WindowResizedEvent(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public int Width { get; }
    public int Height { get; }
    public bool IsHandled { get; set; }
    public EventCategory Category => EventCategory.Application;

    public override string ToString()
    {
        return $"{nameof(WindowResizedEvent)}: {Width}, {Height}";
    }
}