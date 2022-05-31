using Akytos.Assets;
using Akytos.Physics;

namespace Akytos.SceneSystems;

public class CollisionObject2D : Node2D
{
    [SerializeField] private Shape2DAsset? m_shape2D;
    
    public CollisionObject2D()
    {
    }

    public CollisionObject2D(string name)
        : base(name)
    {
    }

    public CollisionObject2D(string name, Shape2DAsset? shape2D) : base(name)
    {
        m_shape2D = shape2D;
    }

    public IShape2D? Shape => m_shape2D?.Data;
}