namespace Akytos.Events;

internal class MouseUpEvent : MouseEvent
{
    public MouseUpEvent(MouseButton mouseButton) : base(mouseButton)
    {
    }
    
    public override string ToString()
    {
        return $"{nameof(MouseUpEvent)}: {MouseButton}";
    }
}