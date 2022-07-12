using System.Runtime.InteropServices;
using SharpDX.Text;
using Veldrid;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Veldrid.SPIRV;
using Encoding = System.Text.Encoding;

namespace Akytos.Graphics;

internal class ResourceFactory : IResourceFactory
{
    private readonly GraphicsDevice m_graphicsDevice;


    public ResourceFactory(GraphicsDevice graphicsDevice)
    {
        m_graphicsDevice = graphicsDevice;
    }

    public Framebuffer CreateFramebuffer(FramebufferDescription framebufferDescription)
    {
        return m_graphicsDevice.ResourceFactory.CreateFramebuffer(framebufferDescription);
    }

    public ShaderProgram CreateShader(string filePath)
    {
        string shaderSource = File.ReadAllText(filePath);
        
        string[] processedSources = PreProcessShaderSource(shaderSource);

        var vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(processedSources[0]), "main");
        var fragmentShaderDesc =
            new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(processedSources[1]), "main");

        var shaders = m_graphicsDevice.ResourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

        if (shaders is null)
        {
            throw new ArgumentException();
        }
        
        return new ShaderProgram(shaders[0], shaders[1]);
    }

    public ShaderProgram CreateShader(Stream fileStream)
    {
        string shaderSource;
        
        using (var streamReader = new StreamReader(fileStream))
        {
            shaderSource = streamReader.ReadToEnd();
        }

        string[] processedSources = PreProcessShaderSource(shaderSource);

        var vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(processedSources[0]), "main");
        var fragmentShaderDesc =
            new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(processedSources[1]), "main");

        var shaders = m_graphicsDevice.ResourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

        if (shaders is null)
        {
            throw new ArgumentException();
        }
        
        return new ShaderProgram(shaders[0], shaders[1]);
    }

    public Texture CreateTexture(int width, int height, PixelFormat pixelFormat, TextureUsage usage)
    {
        var textureDescription = TextureDescription.Texture2D((uint) width, (uint) height, 1, 1, pixelFormat, usage);
        return m_graphicsDevice.ResourceFactory.CreateTexture(textureDescription);
    }

    public Texture CreateTexture(string filePath, TextureUsage usage = TextureUsage.Sampled)
    {
        var image = Image.Load<Rgba32>(filePath);
        
        image.Mutate(x => x.Flip(FlipMode.Vertical));

        var textureDescription = TextureDescription.Texture2D((uint)image.Width, (uint)image.Height, 1, 1,
            PixelFormat.R8_G8_B8_A8_UNorm, usage);
        var texture = m_graphicsDevice.ResourceFactory.CreateTexture(textureDescription);
        image.ProcessPixelRows(accessor =>
        {
            m_graphicsDevice.UpdateTexture(texture, accessor.GetRowSpan(0).ToArray(), 0, 0, 0, (uint)image.Width, (uint)image.Height, 1, 0, 0);
        });

        return texture;
    }
    
    private static string[] PreProcessShaderSource(string sourceString)
    {
        string[] result = new string[2];

        string[] shaderSources = sourceString.Split("#type ");

        for (int i = 0; i < shaderSources.Length; i++)
        {
            string shaderSource = shaderSources[i];
            if (shaderSource == "") continue;

            string newLine = PlatformConstants.NewLine;
            
            string source = shaderSource[(shaderSource.IndexOf(newLine, StringComparison.Ordinal) + 1)..];
            result[i] = source;
        }

        return result;
    }
}