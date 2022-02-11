namespace Akytos.Graphics.Buffers;

public interface IFramebuffer : IGraphicsResource
{
    FrameBufferSpecification Specification { get; }
    uint GetColorAttachmentRendererId(int index = 0);
    int ReadPixel(int attachmentIndex, int x, int y);
    void Bind();
    void Unbind();
    void Resize(uint width, uint height);
    void ClearAttachment(int attachmentIndex, int value);
}