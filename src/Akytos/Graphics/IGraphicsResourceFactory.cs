using Akytos.Graphics.Buffers;

namespace Akytos.Graphics;

public interface IGraphicsResourceFactory
{
    /// <summary>
    ///     Creates a new IBufferObject with the specified data type.
    /// </summary>
    /// <param name="bufferTarget">The type of buffer to create.</param>
    /// <param name="data">The data to store in the buffer object.</param>
    /// <typeparam name="TData">The type of data that should be stored.</typeparam>
    /// <returns></returns>
    IBufferObject<TData> CreateBuffer<TData>(BufferTarget bufferTarget, Span<TData> data) where TData : unmanaged;
    /// <summary>
    ///     Creates a new empty IBufferObject with the specified data type.
    /// </summary>
    /// <param name="bufferTarget">The type of buffer to create.</param>
    /// <param name="length">The total length of the buffer.</param>
    /// <typeparam name="TData">The type of data that should be stored.</typeparam>
    IBufferObject<TData> CreateBuffer<TData>(BufferTarget bufferTarget, int length) where TData : unmanaged;
    /// <summary>
    ///     Creates a new Framebuffer from the provided specifications.
    /// </summary>
    /// <param name="specification">The <see cref="FrameBufferSpecification"/> to create a Framebuffer from.</param>
    /// <returns></returns>
    IFramebuffer CreateFramebuffer(FrameBufferSpecification specification);
    /// <summary>
    ///     Creates a new ShaderProgram.
    /// </summary>
    /// <param name="filePath">Path to the file containing the shader source code.</param>
    /// <returns></returns>
    IShaderProgram CreateShader(string filePath);
    IShaderProgram CreateShader(string name, Stream fileStream);
    /// <summary>
    ///     Creates a new Texture2D from an image file.
    /// </summary>
    /// <param name="filePath">Path to the image file to load.</param>
    /// <returns></returns>
    ITexture2D CreateTexture2D(string filePath);
    /// <summary>
    ///     Creates a new raw Texture2D with the specified byte data, width and height.
    /// </summary>
    /// <param name="data">The byte data to load into the texture.</param>
    /// <param name="width">The desired width of the texture.</param>
    /// <param name="height">The desired height of the texture.</param>
    /// <returns></returns>
    ITexture2D CreateTexture2D(Span<byte> data, int width, int height);
    IVertexArrayObject<TArray, TElement> CreateVertexArray<TArray, TElement>() where TArray : unmanaged where TElement : unmanaged;
}