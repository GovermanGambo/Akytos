namespace Akytos;

internal interface ISceneSystem
{
    bool IsEnabled { get; set; }
    void OnUpdate(DeltaTime time);
    void OnRender();
}