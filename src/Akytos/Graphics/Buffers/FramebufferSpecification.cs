namespace Akytos.Graphics.Buffers;

public class FrameBufferSpecification
{
    public uint Width { get; set; }
    public uint Height { get; set; }
    public FramebufferAttachmentSpecification? Attachments { get; set; }
    public uint Samples { get; set; }
    public bool SwapChainTarget { get; set; }
}