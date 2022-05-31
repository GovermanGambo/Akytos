using System.Numerics;

namespace Akytos.Physics;

public interface IShape2D
{
    bool Collide(Matrix4x4 transform, IShape2D other, Matrix4x4 otherTransform);
    bool CollideWithMotion(Matrix4x4 transform, Vector2 motion, IShape2D other, Matrix4x4 otherTransform);
}