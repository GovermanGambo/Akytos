namespace Akytos.Events;

internal class WindowClosingEvent : IEvent
{
    public bool IsHandled { get; set; }
    public EventCategory Category => EventCategory.Application;

    public override string ToString()
    {
        return $"{nameof(WindowClosingEvent)}";
    }
}