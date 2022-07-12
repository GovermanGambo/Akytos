using System.Numerics;
using Veldrid;

namespace Akytos.Graphics;

public interface ISpriteBatch
{
    void Begin(ICamera camera);
    void End();
    void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation, Color color, int objectId,
        bool centered = false);
}