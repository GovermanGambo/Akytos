using System.Globalization;
using System.Numerics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace Akytos.Serialization.Surrogates;

public class ColorSerializationSurrogate : ISerializationSurrogate<Color>
{
    public void Serialize(IEmitter emitter, object? value)
    {
        Serialize(emitter, (Color)value!);
    }

    public Color Deserialize(Scanner scanner)
    {
        scanner.Read<BlockMappingStart>();
        
        var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";

        string r = scanner.ReadScalar("r");
        string g = scanner.ReadScalar("g");
        string b = scanner.ReadScalar("b");
        string a = scanner.ReadScalar("a");
        
        scanner.Read<BlockEnd>();

        return new Color(float.Parse(r, NumberStyles.Any, ci), float.Parse(g, NumberStyles.Any, ci), float.Parse(b, NumberStyles.Any, ci), float.Parse(a, NumberStyles.Any, ci));
    }

    public void Serialize(IEmitter emitter, Color value)
    {
        emitter.Emit(new MappingStart());
        
        emitter.Emit(new Scalar("r"));
        emitter.Emit(new Scalar(value.R.ToString(CultureInfo.InvariantCulture)));
        emitter.Emit(new Scalar("g"));
        emitter.Emit(new Scalar(value.G.ToString(CultureInfo.InvariantCulture)));
        emitter.Emit(new Scalar("b"));
        emitter.Emit(new Scalar(value.B.ToString(CultureInfo.InvariantCulture)));
        emitter.Emit(new Scalar("a"));
        emitter.Emit(new Scalar(value.A.ToString(CultureInfo.InvariantCulture)));
        
        emitter.Emit(new MappingEnd());
    }

    object ISerializationSurrogate.Deserialize(Scanner value)
    {
        return Deserialize(value);
    }
}