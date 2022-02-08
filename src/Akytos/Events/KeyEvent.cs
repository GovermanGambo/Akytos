namespace Akytos.Events;

internal abstract class KeyEvent : IEvent
{
    protected KeyEvent(KeyCode keyCode)
    {
        KeyCode = keyCode;
    }

    public KeyCode KeyCode { get; }
    public bool IsHandled { get; set; }
    public EventCategory Category => EventCategory.Keyboard;
}