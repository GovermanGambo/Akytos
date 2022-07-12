using Veldrid;

namespace Akytos.Graphics;

public class RenderService
{
    private readonly Pipeline m_pipeline;
    private readonly GraphicsDevice m_graphicsDevice;

    public RenderService(Pipeline pipeline, GraphicsDevice graphicsDevice, CommandList commandList)
    {
        m_pipeline = pipeline;
        m_graphicsDevice = graphicsDevice;
        CommandList = commandList;
    }

    public CommandList CommandList { get; }
}