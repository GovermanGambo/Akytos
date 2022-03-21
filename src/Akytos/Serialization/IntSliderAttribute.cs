namespace Akytos;

public class IntSliderAttribute : SerializeFieldAttribute
{
    public IntSliderAttribute(int min, int max) : base()
    {
        Min = min;
        Max = max;
    }
    
    public IntSliderAttribute(string name, int min, int max) : base(name)
    {
        Min = min;
        Max = max;
    }
    
    public int Min { get; }
    public int Max { get; }
}