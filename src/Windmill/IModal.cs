using System;
using Akytos.Events;

namespace Windmill;

internal interface IModal : IDisposable
{
    string Name { get; }
    bool IsOpen { get; }
    event Action Closing;
    void Open();
    void Close();
    void OnDrawGui();
    void OnEvent(IEvent e);
}