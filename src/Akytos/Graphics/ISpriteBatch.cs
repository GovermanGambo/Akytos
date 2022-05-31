using System.Numerics;

namespace Akytos.Graphics;

public interface ISpriteBatch
{
    void Begin(ICamera camera);
    void End();
    void Draw(ITexture2D texture, Vector2 position, Vector2 scale, float rotation, Color color, int objectId,
        bool centered = false);
}