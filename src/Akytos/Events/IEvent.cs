namespace Akytos.Events;

internal interface IEvent
{ 
    bool IsHandled { get; set;  }
    EventCategory Category { get; }
}