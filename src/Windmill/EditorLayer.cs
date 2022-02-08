using Akytos;
using Akytos.Assets;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Graphics.Buffers;
using Akytos.Layers;

namespace Windmill;

internal class EditorLayer : ILayer
{
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly IGraphicsResourceFactory m_graphicsResourceFactory;

    private IVertexArrayObject<float, uint> m_vertexArrayObject;
    private IShaderProgram m_shaderProgram;

    public EditorLayer(IGraphicsDevice graphicsDevice, IGraphicsResourceFactory graphicsResourceFactory)
    {
        m_graphicsDevice = graphicsDevice;
        m_graphicsResourceFactory = graphicsResourceFactory;
    }

    public void Dispose()
    {
        
    }

    public bool IsEnabled { get; set; } = true;
    public void OnAttach()
    {
        float[] vertices = {
            -0.5f, -0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
             0.5f,  0.5f, 0.0f,
            -0.5f,  0.5f, 0.0f
        };

        var vertexBufferObject = m_graphicsResourceFactory.CreateBuffer<float>(BufferTarget.ArrayBuffer, vertices);

        uint[] indices =
        {
            0, 1, 3, 3, 1, 2
        };

        var elementArrayBufferObject =
            m_graphicsResourceFactory.CreateBuffer<uint>(BufferTarget.ElementArrayBuffer, indices);

        var bufferLayout = new BufferLayout(new List<BufferElement>
        {
            new (ShaderDataType.Float3, "a_Position")
        });

        m_vertexArrayObject = m_graphicsResourceFactory.CreateVertexArray<float, uint>();
        m_vertexArrayObject.SetBufferLayout(bufferLayout);
        m_vertexArrayObject.AddArrayBuffer(vertexBufferObject);
        m_vertexArrayObject.SetElementArrayBuffer(elementArrayBufferObject);

        m_shaderProgram =
            m_graphicsResourceFactory.CreateShader(Path.Combine(Asset.GetAssetPath(), "shaders/Red.glsl"));
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        m_graphicsDevice.ClearColor(new Color(0.1f, 0.1f, 0.1f));
        m_graphicsDevice.Clear();
        
        m_shaderProgram.Bind();
        m_graphicsDevice.DrawIndexed(m_vertexArrayObject);
        m_shaderProgram.Unbind();
    }

    public void OnEvent(IEvent e)
    {
    }
}