namespace Akytos;

public class IntSliderAttribute : SerializeFieldAttribute
{
    public IntSliderAttribute(string name, int min, int max) : base(name)
    {
        Min = min;
        Max = max;
    }
    
    public int Min { get; }
    public int Max { get; }
}