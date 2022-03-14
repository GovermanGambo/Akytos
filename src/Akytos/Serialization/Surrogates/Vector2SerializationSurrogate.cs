using System.Globalization;
using System.Numerics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Akytos.Surrogates;

public class Vector2SerializationSurrogate : ISerializationSurrogate<Vector2>
{
    public void Serialize(IEmitter emitter, object? value)
    {
        Serialize(emitter, (Vector2)value!);
    }

    public Vector2 Deserialize(string value)
    {
        throw new NotImplementedException();
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

    object ISerializationSurrogate.Deserialize(string value)
    {
        return Deserialize(value);
    }
}