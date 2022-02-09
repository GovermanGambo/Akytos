using System.Collections;

namespace Akytos.Graphics.Buffers;

internal class BufferLayout : IEnumerable<BufferElement>
{
    private readonly List<BufferElement> m_bufferElements;

    public BufferLayout(IEnumerable<BufferElement> bufferElements)
    {
        m_bufferElements = bufferElements.ToList();

        foreach (var bufferElement in m_bufferElements)
        {
            bufferElement.Offset = Stride;
            Stride += bufferElement.Size;
        }
    }
    
    public int Stride { get; }
    public IEnumerator<BufferElement> GetEnumerator()
    {
        return m_bufferElements.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}