namespace Akytos;

public class SerializeFieldAttribute : Attribute
{
    public SerializeFieldAttribute(string name)
    {
        Name = name;
    }

    public SerializeFieldAttribute(string name, SpecialField specialField)
    {
        Name = name;
        SpecialField = specialField;
    }

    public string Name { get; }
    public SpecialField? SpecialField { get; }
}