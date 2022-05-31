namespace Akytos.SceneSystems;

public class CanvasItem : Node
{
    [SerializeField] private Color m_modulate = Color.White;
    [SerializeField] private bool m_isVisible = true;
    
    public CanvasItem() : base("NewCanvasItem")
    {
    }

    public CanvasItem(string name)
        : base(name)
    {
    }

    public Color Modulate
    {
        get => m_modulate;
        set => m_modulate = value;
    }

    public bool IsVisible
    {
        get => m_isVisible;
        set => m_isVisible = value;
    }
}