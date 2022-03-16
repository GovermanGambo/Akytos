using System;
using Akytos;

namespace Windmill;

public interface IModal : IDisposable
{
    string Name { get; }
    bool IsOpen { get; set; }
    void OnAppearing();
    void OnDrawGui(DeltaTime deltaTime);
}