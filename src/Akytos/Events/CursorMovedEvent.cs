namespace Akytos.Events;

internal class CursorMovedEvent : IEvent
{
    public CursorMovedEvent(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float X { get; }
    public float Y { get; }
    public bool IsHandled { get; set; }
    public EventCategory Category => EventCategory.Mouse;

    public override string ToString()
    {
        return $"{nameof(CursorMovedEvent)}: {X}, {Y}";
    }
}