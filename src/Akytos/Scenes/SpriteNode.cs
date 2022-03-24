using System.Numerics;
using Akytos.Assets;
using Akytos.Graphics;

namespace Akytos;

public class SpriteNode : Node2D
{
    [SerializeField("Texture")] private Texture2DAsset? m_textureAsset;
    [SerializeField] private bool m_isCentered;

    public SpriteNode() : base("NewSpriteNode")
    {
    }
    
    public SpriteNode(string name) : base(name)
    {
    }

    public bool IsCentered
    {
        get => m_isCentered;
        set => m_isCentered = value;
    }

    public bool FlipHorizontal { get; set; }
    public bool FlipVertical { get; set; }
    public int Frame { get; set; }
    public Vector2 FrameCoordinates { get; set; }
    public int HorizontalFrames { get; set; } = 1;
    public int VerticalFrames { get; set; } = 1;
    public Vector2 Offset { get; set; }
    public ITexture2D? Texture => m_textureAsset?.Data;
}