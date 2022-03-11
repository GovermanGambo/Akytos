using Akytos;

namespace Windmill;

public interface IModal : IDisposable
{
    bool IsOpen { get; internal set; }
    void OnAppearing();
    void OnDrawGui(DeltaTime deltaTime);
}