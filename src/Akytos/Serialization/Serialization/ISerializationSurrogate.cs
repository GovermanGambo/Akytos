using YamlDotNet.Core;

namespace Akytos.Serialization;

public interface ISerializationSurrogate<T> : ISerializationSurrogate
{
    void Serialize(IEmitter emitter, T? value);
    new T Deserialize(string value);
}

public interface ISerializationSurrogate
{
    void Serialize(IEmitter emitter, object? value);
    object Deserialize(string value);
}