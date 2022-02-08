namespace Akytos.Events;

internal class KeyUpEvent : KeyEvent
{
    public KeyUpEvent(KeyCode keyCode) : base(keyCode)
    {
    }

    public override string ToString()
    {
        return $"{nameof(KeyUpEvent)}: {KeyCode}";
    }
}