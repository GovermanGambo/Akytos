namespace Akytos.Events;

internal class KeyDownEvent : KeyEvent
{
    public KeyDownEvent(KeyCode keyCode, int repeatCount = 1) : base(keyCode)
    {
        RepeatCount = repeatCount;
    }
    
    public int RepeatCount { get; }

    public override string ToString()
    {
        return $"{nameof(KeyDownEvent)}: {KeyCode} ({RepeatCount} repeats)";
    }
}