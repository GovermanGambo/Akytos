namespace Akytos.Graphics.Buffers;

public class FramebufferAttachmentSpecification
{
    public FramebufferAttachmentSpecification(List<FramebufferTextureSpecification> attachments)
    {
        Attachments = attachments;
    }

    public List<FramebufferTextureSpecification> Attachments { get; }
}