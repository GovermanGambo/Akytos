using System.Globalization;

namespace Akytos;

public readonly struct DeltaTime
{
    public static implicit operator float(DeltaTime deltaTime) => deltaTime.Seconds;

    public DeltaTime(float seconds)
    {
        Seconds = seconds;
    }

    public float Seconds { get; }

    public float Milliseconds => Seconds * 1000.0f;

    public override string ToString()
    {
        return Seconds.ToString(CultureInfo.InvariantCulture);
    }
}