using System.Numerics;
using Akytos;
using Akytos.Assets;
using Akytos.SceneSystems;

namespace Pong.Nodes;

public class Ball : PhysicsObject2D
{
    [SerializeField] private float m_speed = 1.0f;

    public Ball()
    {
    }

    public Ball(string name)
        : base(name)
    {
    }

    public Ball(string name, Shape2DAsset shape)
        : base(name, shape)
    {
    }

    public float Speed
    {
        get => m_speed;
        set => m_speed = value;
    }
    
    public override void OnReady()
    {
        var random = new Random();
        float x = random.Next(0, 1) == 1 ? 1.0f : -1.0f;
        float y = random.NextSingle() * 2.0f - 1.0f;

        Velocity = Vector2.Normalize(new Vector2(x, y)) * Speed;
    }

    public void Bounce()
    {
        var random = new Random();
        float x = Velocity.X * -1.0f;
        float y = random.NextSingle() * 2.0f - 1.0f;

        Velocity = Vector2.Normalize(new Vector2(x, y)) * Speed;
    }
}