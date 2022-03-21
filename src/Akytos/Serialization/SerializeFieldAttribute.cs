namespace Akytos;

public class SerializeFieldAttribute : Attribute
{
    public SerializeFieldAttribute(string? name = null)
    {
        Name = name;
    }

    public string? Name { get; }
}