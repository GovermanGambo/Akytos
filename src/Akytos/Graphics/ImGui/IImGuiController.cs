namespace Akytos.Graphics;

internal interface IImGuiController : IDisposable
{
    void Render();
    void Update(float deltaSeconds);
}