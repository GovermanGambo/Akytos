namespace Akytos.Graphics.Buffers;

internal class BufferElement
{
    public BufferElement(ShaderDataType dataType, string name, bool isNormalized = false)
    {
        DataType = dataType;
        Name = name;
        IsNormalized = isNormalized;

        Size = dataType.GetSize();
        ComponentCount = dataType.GetComponentCount();
    }

    public int ComponentCount { get; }

    public ShaderDataType DataType { get; }
    public string Name { get; }
    public int Size { get; }
    public int Offset { get; set; }
    public bool IsNormalized { get; }
}