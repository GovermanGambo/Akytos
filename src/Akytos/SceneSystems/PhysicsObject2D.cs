using System.Numerics;
using Akytos.Assets;

namespace Akytos.SceneSystems;

public class PhysicsObject2D : CollisionObject2D
{
    public PhysicsObject2D()
    {
    }

    public PhysicsObject2D(string name)
        : base(name)
    {
    }

    public PhysicsObject2D(string name, Shape2DAsset shape)
        : base(name, shape)
    {
    }
    
    public Vector2 Velocity { get; set; }
}