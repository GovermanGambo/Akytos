using Akytos.Assets;
using Akytos.Graphics;
using Veldrid;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace Akytos.Serialization.Surrogates;

internal class Texture2DAssetSerializationSurrogate : ISerializationSurrogate<Asset<Texture>?>
{
    private readonly IAssetManager m_assetManager;

    public Texture2DAssetSerializationSurrogate(IAssetManager assetManager)
    {
        m_assetManager = assetManager;
    }

    public Asset<Texture>? Deserialize(Scanner scanner)
    {
        scanner.Read<BlockMappingStart>();

        string path = scanner.ReadScalar("path");

        scanner.Read<BlockEnd>();
        
        if (path == string.Empty)
        {
            return null;
        }

        return m_assetManager.Load<Texture>(path);
    }

    public void Serialize(IEmitter emitter, Asset<Texture>? value)
    {
        emitter.Emit(new MappingStart());
        
        emitter.Emit(new Scalar("path"));
        emitter.Emit(new Scalar(value?.FilePath ?? ""));
        
        emitter.Emit(new MappingEnd());
    }
    
    public void Serialize(IEmitter emitter, object? value)
    {
        Serialize(emitter, value as Asset<Texture>);
    }

    object ISerializationSurrogate.Deserialize(Scanner value)
    {
        // TODO: Need to figure out how null / non-null serializable stuff should work.
        return Deserialize(value);
    }
}