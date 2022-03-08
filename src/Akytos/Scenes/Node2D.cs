using System.Numerics;

namespace Akytos
{
    public class Node2D : Node
    {
        [SerializeField("Position")] private Vector2 position;
        [SerializeField("Rotation")] private float rotation;
        [SerializeField("Scale")] private Vector2 scale = Vector2.One;

        public Node2D(string name) : base(name)
        {
        }

        public Vector2 GlobalPosition
        {
            get
            {
                if (Owner is not Node2D node2D)
                {
                    return Position;
                }

                return Position + node2D.GlobalPosition;
            }
            set
            {
                if (Owner is not Node2D node2D)
                {
                    Position = value;
                }
                else
                {
                    Position = value - node2D.GlobalPosition;
                }
            }
        }

        public float GlobalRotation
        {
            get
            {
                if (Owner is not Node2D node2D)
                {
                    return Rotation;
                }

                return Rotation + node2D.GlobalRotation;
            }
            set
            {
                if (Owner is not Node2D node2D)
                {
                    Rotation = value;
                }
                else
                {
                    Rotation = value - node2D.GlobalRotation;
                }
            }
        }

        public float GlobalRotationDegrees
        {
            get => Math.Degrees(GlobalRotation);
            set => GlobalRotation = Math.Radians(value);
        }

        public Vector2 GlobalScale
        {
            get
            {
                if (Owner is not Node2D node2D)
                {
                    return Scale;
                }

                return Scale * node2D.GlobalScale;
            }
            set
            {
                if (Owner is not Node2D node2D)
                {
                    Scale = value;
                }
                else
                {
                    Scale = value / node2D.GlobalScale;
                }
            }
        }
        
        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public float Rotation
        {
            get => rotation;
            set => rotation = value;
        }
        
        public float RotationDegrees
        {
            get => Math.Degrees(Rotation);
            set => Rotation = Math.Radians(value);
        }
        
        public Vector2 Scale
        {
            get => scale;
            set => scale = value;
        }

        public int ZIndex { get; set; }
    }
}