namespace Akytos.Events;

internal abstract class MouseEvent : IEvent
{
    protected MouseEvent(MouseButton mouseButton)
    {
        MouseButton = mouseButton;
    }

    public MouseButton MouseButton { get; }
    public bool IsHandled { get; set; }
    public EventCategory Category => EventCategory.Mouse;
}