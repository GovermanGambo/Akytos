namespace Akytos.Events;

internal class EventDispatcher
{
    private readonly IEvent m_event;

    public EventDispatcher(IEvent @event)
    {
        m_event = @event;
    }
    
    public void Dispatch<TEvent>(Func<TEvent, bool> eventCallback, Func<bool> canExecute) where TEvent : IEvent
    {
        if (!canExecute())
        {
            return;
        }
        
        Dispatch(eventCallback);
    }

    public void Dispatch<TEvent>(Func<TEvent, bool> eventCallback) where TEvent : IEvent
    {
        if (m_event.IsHandled || m_event is not TEvent @event) return;
        
        bool result = eventCallback(@event);
        @event.IsHandled = result;
    }
}