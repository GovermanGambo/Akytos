using System;

namespace Windmill;

public interface IModal : IDisposable
{
    string Name { get; }
    bool IsOpen { get; }
    void Show();
    void Hide();
    void OnDrawGui();
}