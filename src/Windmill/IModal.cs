using System;

namespace Windmill;

public interface IModal : IDisposable
{
    string Name { get; }
    bool IsOpen { get; set; }
    void OnDrawGui();
}