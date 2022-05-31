using System.Numerics;

namespace Akytos.Physics;

public class RectangleShape2D : IShape2D
{
    public Vector2 Extents { get; set; } = new(10, 10);
    
    public bool Collide(Matrix4x4 transform, IShape2D other, Matrix4x4 otherTransform)
    {
        return other switch
        {
            RectangleShape2D rectangle => CollideWithRectangle(transform, rectangle, otherTransform),
            _ => false
        };
    }

    public bool CollideWithMotion(Matrix4x4 transform, Vector2 motion, IShape2D other, Matrix4x4 otherTransform)
    {
        return other switch
        {
            RectangleShape2D rectangle => CollideWithMotionRectangle(transform, motion, rectangle, otherTransform),
            _ => false
        };
    }

    private bool CollideWithRectangle(Matrix4x4 transform, IShape2D other, Matrix4x4 otherTransform)
    {
        
        return false;
    }
    
    private bool CollideWithMotionRectangle(Matrix4x4 transform, Vector2 motion, IShape2D other, Matrix4x4 otherTransform)
    {
        
        return false;
    }
    
}