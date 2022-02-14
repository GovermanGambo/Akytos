namespace Akytos;

public class SerializeFieldAttribute : Attribute
{
    public SerializeFieldAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}