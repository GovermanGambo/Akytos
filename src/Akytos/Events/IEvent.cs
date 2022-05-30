namespace Akytos.Events;

public interface IEvent
{ 
    bool IsHandled { get; set;  }
    EventCategory Category { get; }
}