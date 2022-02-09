using Akytos.Graphics.Buffers;

namespace Akytos.Graphics;

internal interface IVertexArrayObject : IGraphicsResource
{
    int ElementCount { get; }
    void Bind();
    void SetBufferLayout(BufferLayout bufferLayout);
}

internal interface IVertexArrayObject<TArray, TElement> : IVertexArrayObject
    where TArray : unmanaged where TElement : unmanaged
{
    
    void AddArrayBuffer(IBufferObject<TArray> arrayBuffer);
    void SetElementArrayBuffer(IBufferObject<TElement> elementArrayBuffer);

}