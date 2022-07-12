using System.Numerics;
using Veldrid;

namespace Akytos.Graphics;

public static class SpriteBatchExtensions
{
    public static void Draw(this ISpriteBatch spriteBatch, Texture texture2D, Vector2 position, int objectId)
    {
        spriteBatch.Draw(texture2D, position, Vector2.One, 0f, Color.White, objectId);
    }
}