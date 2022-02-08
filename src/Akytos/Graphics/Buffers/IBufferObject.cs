namespace Akytos.Graphics.Buffers;

public interface IBufferObject<TData> : IGraphicsResource where TData : unmanaged
{
    int Length { get; }
    void Bind();
    void Unbind();
    void SetData(Span<TData> data);
}