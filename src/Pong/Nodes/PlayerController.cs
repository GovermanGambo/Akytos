using System.Numerics;
using Akytos;
using Akytos.SceneSystems;

namespace Pong.Nodes;

public class PlayerController : Node2D
{
    private readonly ControlScheme m_controlScheme;

    public PlayerController(string name, ControlScheme controlScheme)
        : base(name)
    {
        m_controlScheme = controlScheme;
    }

    public float Speed { get; set; } = 1.0f;
    
    public override void OnUpdate(float deltaSeconds)
    {
        if (m_controlScheme is ControlScheme.Arrows)
        {
            if (Input.GetKeyPressed(KeyCode.Up))
            {
                Position += Vector2.UnitY * Speed * deltaSeconds;
            }
            else if (Input.GetKeyPressed(KeyCode.Down))
            {
                Position -= Vector2.UnitY * Speed * deltaSeconds;
            }
        }
        else if (m_controlScheme is ControlScheme.WASD)
        {
            if (Input.GetKeyPressed(KeyCode.W))
            {
                Position += Vector2.UnitY * Speed * deltaSeconds;
            }
            else if (Input.GetKeyPressed(KeyCode.S))
            {
                Position -= Vector2.UnitY * Speed * deltaSeconds;
            }
        }
    }
}

public enum ControlScheme
{
    Arrows,
    WASD
}