using Akytos.Graphics.Buffers;

namespace Akytos.Graphics;

public interface IVertexArrayObject : IGraphicsResource
{
    int ElementCount { get; }
    void Bind();
    void SetBufferLayout(BufferLayout bufferLayout);
}

public interface IVertexArrayObject<TArray, TElement> : IVertexArrayObject
    where TArray : unmanaged where TElement : unmanaged
{
    void AddArrayBuffer(IBufferObject<TArray> arrayBuffer);
    void SetElementArrayBuffer(IBufferObject<TElement> elementArrayBuffer);

}