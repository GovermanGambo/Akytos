using System.Numerics;

namespace Akytos.SceneSystems
{
    public class Node2D : CanvasItem
    {
        [SerializeField] private Vector2 position;
        [SerializeField] private Vector2 scale = Vector2.One;
        [IntSlider(0, 360)] private int rotation;

        public Node2D() : base("NewNode2D")
        {
            
        }
        
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
            get => Math.Radians(RotationDegrees);
            set => RotationDegrees = (int)Math.Degrees(value);
        }
        
        public int RotationDegrees
        {
            get => rotation;
            set => rotation = value;
        }
        
        public Vector2 Scale
        {
            get => scale;
            set => scale = value;
        }

        public int ZIndex { get; set; }
    }
}