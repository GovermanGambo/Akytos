using System.Numerics;
using System.Reflection;
using Akytos.Assets;
using Akytos.Graphics.Buffers;

namespace Akytos.Graphics;

public class SpriteBatch : ISpriteBatch
{
    private const int MaxQuads = 10000;
    private const int MaxVertices = MaxQuads * 4;
    private const int MaxElements = MaxQuads * 6;
    private const int MaxTextureSlots = 32;

    private readonly IGraphicsDevice m_graphicsDevice;

    private readonly QuadVertex[] m_quadVertices;
    private readonly IVertexArrayObject<float, uint> m_quadVertexArray;
    private readonly IBufferObject<float> m_quadVertexBuffer;
    private readonly IShaderProgram m_textureShader;
    private readonly ITexture2D[] m_textureSlots;
    private readonly Vector3[] m_quadVertexPositions = new Vector3[4];
    private readonly Vector3[] m_centeredQuadVertexPositions = new Vector3[4];

    private int m_quadElementCount;
    private int m_quadVertexCount;
    private int m_textureSlotIndex;

    public SpriteBatch(IGraphicsDevice graphicsDevice, IGraphicsResourceFactory graphicsResourceFactory)
    {
        m_graphicsDevice = graphicsDevice;

        m_quadVertexArray = graphicsResourceFactory.CreateVertexArray<float, uint>();
        m_quadVertexArray.SetBufferLayout(new BufferLayout(new List<BufferElement>
        {
            new(ShaderDataType.Float3, "a_Position"),
            new(ShaderDataType.Float4, "a_Color"),
            new(ShaderDataType.Float2, "a_UV"),
            new(ShaderDataType.Int, "a_TextureIndex"),
            new(ShaderDataType.Int, "a_ObjectId")
        }));
        
        m_quadVertexBuffer =
            graphicsResourceFactory.CreateBuffer<float>(BufferTarget.ArrayBuffer, MaxVertices * 10);
        m_quadVertexArray.AddArrayBuffer(m_quadVertexBuffer);

        m_quadVertices = new QuadVertex[MaxVertices];

        uint[] indices = CreateElements();
        var quadElementBuffer = graphicsResourceFactory.CreateBuffer<uint>(BufferTarget.ElementArrayBuffer, indices);
        m_quadVertexArray.SetElementArrayBuffer(quadElementBuffer);

        var whiteTexture = graphicsResourceFactory.CreateTexture2D(new Span<byte>(new byte[] { 255, 255, 255, 255 }), 1, 1);

        int[] samplers = new int[MaxTextureSlots];
        for (int i = 0; i < MaxTextureSlots; i++)
        {
            samplers[i] = i;
        }

        using (var stream = Assembly.GetExecutingAssembly()
                   .GetManifestResourceStream("Akytos.Resources.Shaders.Sprites_Default.glsl"))
        {
            if (stream is not null)
            {
                m_textureShader = graphicsResourceFactory.CreateShader("Sprites_Default.glsl", stream);
            }
        }

        m_textureShader.Bind();
        m_textureShader.SetIntArray("u_Textures", samplers);

        m_textureSlots = new ITexture2D[MaxTextureSlots];
        m_textureSlots[0] = whiteTexture;
        
        m_centeredQuadVertexPositions[0] = new Vector3(-0.5f, -0.5f, 0.0f);
        m_centeredQuadVertexPositions[1] = new Vector3(0.5f, -0.5f, 0.0f);
        m_centeredQuadVertexPositions[2] = new Vector3(0.5f, 0.5f, 0.0f);
        m_centeredQuadVertexPositions[3] = new Vector3(-0.5f, 0.5f, 0.0f);
        
        m_quadVertexPositions[0] = new Vector3(0.0f, -1.0f, 0.0f);
        m_quadVertexPositions[1] = new Vector3(1.0f, -1.0f, 0.0f);
        m_quadVertexPositions[2] = new Vector3(1.0f, 0.0f, 0.0f);
        m_quadVertexPositions[3] = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void Begin(ICamera camera)
    {
        var viewProjection = camera.ViewMatrix * camera.ProjectionMatrix;
        
        m_textureShader.Bind();
        m_textureShader.SetMat4("u_ViewProjection", viewProjection);

        m_quadElementCount = 0;
        m_quadVertexCount = 0;

        m_textureSlotIndex = 1;
        m_quadVertexArray.Bind();
        
    }

    public void Draw(ITexture2D texture2D, Vector2 position, Vector2 scale, float rotation, Color color, int objectId, bool centered = false)
    {
        if (m_quadElementCount > MaxElements)
        {
            FlushAndReset();
        }

        Vector2[] uv =
        {
            new(0.0f, 0.0f),
            new(1.0f, 0.0f),
            new(1.0f, 1.0f),
            new(0.0f, 1.0f)
        };

        int textureIndex = 0;
        for (int i = 1; i < m_textureSlotIndex; i++)
        {
            if (m_textureSlots[i] != texture2D) continue;

            textureIndex = i;
            break;
        }

        if (textureIndex == 0)
        {
            textureIndex = m_textureSlotIndex;
            m_textureSlots[textureIndex] = texture2D;
            m_textureSlotIndex++;
        }

        var rotationMatrix = Matrix4x4.CreateRotationZ(rotation);

        var transform = Matrix4x4.CreateScale(new Vector3(texture2D.Width, texture2D.Height, 1.0f) * new Vector3(scale, 1.0f))
                        * rotationMatrix
                        * Matrix4x4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));
                        

        for (int i = 0; i < 4; i++)
        {
            var vertex = CreateQuadVertex(i, transform, color, textureIndex, uv, objectId, centered);
            m_quadVertices[m_quadVertexCount] = vertex;
            m_quadVertexCount++;
        }

        m_quadElementCount += 6;
    }
    
    public unsafe void End()
    {
        int size = m_quadVertexCount * sizeof(QuadVertex);

        fixed (QuadVertex* d = &m_quadVertices[0])
        {
            m_quadVertexBuffer.SetData(d, (uint)size);
        }

        Flush();
    }

    private QuadVertex CreateQuadVertex(int index, Matrix4x4 transform, Color color, int textureIndex, Vector2[] uv,
        int objectId, bool centered)
    {
        var basePosition = centered ? m_centeredQuadVertexPositions[index] : m_quadVertexPositions[index];
        
        return new QuadVertex(
            Vector3.Transform(basePosition, transform),
            color,
            uv[index],
            textureIndex,
            objectId
        );
    }

    private void FlushAndReset()
    {
        End();
        m_quadElementCount = 0;
        m_quadVertexCount = 0;
        m_textureSlotIndex = 1;
    }

    private void Flush()
    {
        for (int i = 0; i < m_textureSlotIndex; i++)
        {
            m_textureSlots[i].Bind(i);
        }
        
        m_graphicsDevice.DrawIndexed(m_quadVertexArray, m_quadElementCount);
    }

    private static uint[] CreateElements()
    {
        uint[] indices = new uint[MaxElements];
        uint offset = 0;
        for (uint i = 0; i < MaxElements; i += 6)
        {
            indices[i + 0] = offset + 0;
            indices[i + 1] = offset + 1;
            indices[i + 2] = offset + 2;

            indices[i + 3] = offset + 2;
            indices[i + 4] = offset + 3;
            indices[i + 5] = offset + 0;

            offset += 4;
        }

        return indices;
    }

    private readonly struct QuadVertex
    {
        public QuadVertex(Vector3 position, Color color, Vector2 uv, int textureIndex, int objectId)
        {
            Position = position;
            Color = color;
            UV = uv;
            TextureIndex = textureIndex;
            ObjectId = BitConverter.ToSingle(BitConverter.GetBytes(objectId), 0);
        }

        public Vector3 Position { get; }
        public Color Color { get; }
        public Vector2 UV { get; }
        public int TextureIndex { get; }
        public float ObjectId { get; }
    }
}