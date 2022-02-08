namespace Akytos.Events;

internal class MouseDownEvent : MouseEvent
{
    public MouseDownEvent(MouseButton mouseButton) : base(mouseButton)
    {
    }

    public override string ToString()
    {
        return $"{nameof(MouseDownEvent)}: {MouseButton}";
    }
}