using Akytos.Graphics;

namespace Akytos.SceneSystems;

public interface ISceneSystem
{
    bool IsEnabled { get; set; }
    void OnUpdate(DeltaTime time);
    void OnRender(ICamera camera);
}