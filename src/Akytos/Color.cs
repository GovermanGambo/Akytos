using System.Numerics;

namespace Akytos;

/// <summary>
///     Represents an RGBA color instance.
/// </summary>
public readonly struct Color
{
    public static Color Red => new(1.0f, 0.0f, 0.0f);
    public static Color Green => new(0.0f, 1.0f, 0.0f);
    public static Color Blue => new(0.0f, 0.0f, 1.0f);
    public static Color Yellow => new(1.0f, 1.0f, 0.0f);
    public static Color Cyan => new(0.0f, 1.0f, 1.0f);
    public static Color Pink => new(1.0f, 0.0f, 1.0f);
    public static Color White => new(1.0f, 1.0f, 1.0f);

    public static explicit operator Vector4(Color color) => new(color.R, color.G, color.B, color.A);
    public static explicit operator Color(Vector4 vector) => new(vector.X, vector.Y, vector.Z, vector.W);

    public Color(Vector4 vector)
    {
        R = vector.X;
        G = vector.Y;
        B = vector.Z;
        A = vector.W;
    }
    
    public Color(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    ///     Red channel strength
    /// </summary>
    public float R { get; }
    /// <summary>
    ///     Green channel strength
    /// </summary>
    public float G { get; }
    /// <summary>
    ///     Blue channel strength
    /// </summary>
    public float B { get; }
    /// <summary>
    ///     Alpha channel strength
    /// </summary>
    public float A { get; }
}