using Akytos.Assets;
using Akytos.Graphics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace Akytos.Serialization.Surrogates;

internal class Texture2DAssetSerializationSurrogate : ISerializationSurrogate<Texture2DAsset?>
{
    private readonly AssetManager m_assetManager;

    public Texture2DAssetSerializationSurrogate(AssetManager assetManager)
    {
        m_assetManager = assetManager;
    }

    public Texture2DAsset? Deserialize(Scanner scanner)
    {
        scanner.Read<BlockMappingStart>();

        string path = scanner.ReadScalar("path");

        scanner.Read<BlockEnd>();
        
        if (path == string.Empty)
        {
            return null;
        }

        return m_assetManager.Load<ITexture2D>(path) as Texture2DAsset;
    }

    public void Serialize(IEmitter emitter, Texture2DAsset? value)
    {
        emitter.Emit(new MappingStart());
        
        emitter.Emit(new Scalar("path"));
        emitter.Emit(new Scalar(value?.FilePath ?? ""));
        
        emitter.Emit(new MappingEnd());
    }
    
    public void Serialize(IEmitter emitter, object? value)
    {
        Serialize(emitter, value as Texture2DAsset);
    }

    object ISerializationSurrogate.Deserialize(Scanner value)
    {
        // TODO: Need to figure out how null / non-null serializable stuff should work.
        return Deserialize(value);
    }
}