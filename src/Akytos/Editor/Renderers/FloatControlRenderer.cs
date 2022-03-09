#if AKYTOS_EDITOR

namespace Akytos.Editor.Renderers;

internal class FloatControlRenderer : IGuiControlRenderer<float?>
{
    public float? DrawControl(string label, float? value)
    {
        if (AkGui.InputFloat(label, ref value))
        {
            return value;
        }

        return null;
    }

    public object? DrawControl(string label, object? value)
    {
        return DrawControl(label, value as float?);
    }
}

#endif