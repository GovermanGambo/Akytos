using Akytos.Graphics;

namespace Akytos.SceneSystems;

public interface ISystemsRegistry
{ 
    TSystem Register<TSystem>(bool enable = true) where TSystem : ISceneSystem;

    void OnUpdate(DeltaTime deltaTime);

    void OnRender(ICamera camera);
}