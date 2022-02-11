namespace Akytos;

public static class Math
{
    public static float Radians(float value)
    {
        return MathF.PI / 180 * value;
    }

    public static float Degrees(float value)
    {
        return value / (MathF.PI / 180);
    }
}