using Akytos;
using Akytos.Events;
using Akytos.Layers;

namespace Pong;

public class GameLayer : ILayer
{
    public void Dispose()
    {
        
    }

    public bool IsEnabled { get; set; } = true;
    public void OnAttach()
    {
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
    }

    public void OnEvent(IEvent e)
    {
    }

    public void OnDrawGui()
    {
    }
}