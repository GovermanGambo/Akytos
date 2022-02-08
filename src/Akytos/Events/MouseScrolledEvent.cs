namespace Akytos.Events;

internal class MouseScrolledEvent : IEvent
{
    public MouseScrolledEvent(float horizontal, float vertical)
    {
        Horizontal = horizontal;
        Vertical = vertical;
    }

    public float Horizontal { get; }
    public float Vertical { get; }
    public bool IsHandled { get; set; }
    public EventCategory Category => EventCategory.Mouse;

    public override string ToString()
    {
        return $"{nameof(MouseScrolledEvent)}: {Horizontal}, {Vertical}";
    }
}