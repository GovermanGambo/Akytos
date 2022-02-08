using Akytos.Graphics.Buffers;

namespace Akytos.Graphics;

public interface IGraphicsResourceFactory
{
    IBufferObject<TData> CreateBuffer<TData>(BufferTarget bufferTarget, Span<TData> data) where TData : unmanaged;
    IBufferObject<TData> CreateBuffer<TData>(BufferTarget bufferTarget, int size) where TData : unmanaged;
    IShaderProgram CreateShader(string filePath);
    IVertexArrayObject<TArray, TElement> CreateVertexArray<TArray, TElement>() where TArray : unmanaged where TElement : unmanaged;
}