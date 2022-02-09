namespace Akytos.Graphics;

public interface IImGuiController : IDisposable
{
    void Render();
    void Update(float deltaSeconds);
}