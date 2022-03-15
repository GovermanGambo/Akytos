using System.Globalization;
using System.Numerics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace Akytos.Serialization.Surrogates;

public class Vector2SerializationSurrogate : ISerializationSurrogate<Vector2>
{
    public void Serialize(IEmitter emitter, object? value)
    {
        Serialize(emitter, (Vector2)value!);
    }

    public Vector2 Deserialize(Scanner scanner)
    {
        scanner.Read<BlockMappingStart>();

        string x = scanner.ReadScalar("x");
        string y = scanner.ReadScalar("y");
        
        scanner.Read<BlockEnd>();

        return new Vector2(float.Parse(x), float.Parse(y));
    }

    object ISerializationSurrogate.Deserialize(Scanner value)
    {
        return Deserialize(value);
    }

    public void Serialize(IEmitter emitter, Vector2 value)
    {
        emitter.Emit(new MappingStart());
        
        emitter.Emit(new Scalar("x"));
        emitter.Emit(new Scalar(value.X.ToString(CultureInfo.InvariantCulture)));
        emitter.Emit(new Scalar("y"));
        emitter.Emit(new Scalar(value.Y.ToString(CultureInfo.InvariantCulture)));
        
        emitter.Emit(new MappingEnd());
    }
    
}