using Akytos;
using Akytos.Assets;
using Akytos.Editor;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Graphics.Buffers;
using Akytos.Layers;
using ImGuiNET;

namespace Windmill;

internal class EditorLayer : ILayer
{
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly IGraphicsResourceFactory m_graphicsResourceFactory;
    private readonly IEditorViewport m_editorViewport;

    private IVertexArrayObject<float, uint> m_vertexArrayObject;
    private IShaderProgram m_shaderProgram;
    private ITexture2D m_texture2D;

    public EditorLayer(IGraphicsDevice graphicsDevice, IGraphicsResourceFactory graphicsResourceFactory, IEditorViewport editorViewport)
    {
        m_graphicsDevice = graphicsDevice;
        m_graphicsResourceFactory = graphicsResourceFactory;
        m_editorViewport = editorViewport;
    }

    public void Dispose()
    {
        
    }

    public bool IsEnabled { get; set; } = true;
    public void OnAttach()
    {
        float[] vertices = {
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f,
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f
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
            new (ShaderDataType.Float3, "a_Position"),
            new (ShaderDataType.Float2, "a_UV")
        });

        m_vertexArrayObject = m_graphicsResourceFactory.CreateVertexArray<float, uint>();
        m_vertexArrayObject.SetBufferLayout(bufferLayout);
        m_vertexArrayObject.AddArrayBuffer(vertexBufferObject);
        m_vertexArrayObject.SetElementArrayBuffer(elementArrayBufferObject);

        m_shaderProgram =
            m_graphicsResourceFactory.CreateShader(Asset.GetAssetPath("shaders/Texture.glsl"));

        m_texture2D = m_graphicsResourceFactory.CreateTexture2D(Asset.GetAssetPath("sprites/character_malePerson_idle.png"));
        m_shaderProgram.Bind();
        m_shaderProgram.SetInt("u_Texture", 0);
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        var viewProjection = m_editorViewport.Camera.ProjectionMatrix;
        m_shaderProgram.SetMat4("u_ViewProjection", viewProjection);
        
        m_graphicsDevice.ClearColor(new Color(0.1f, 0.1f, 0.1f));
        m_graphicsDevice.Clear();
        
        m_texture2D.Bind();
        
        m_shaderProgram.Bind();
        m_graphicsDevice.DrawIndexed(m_vertexArrayObject);
        m_shaderProgram.Unbind();
    }

    public void OnEvent(IEvent e)
    {;
    }

    public void OnImGui()
    {
        ImGui.ShowDemoWindow();
    }
}