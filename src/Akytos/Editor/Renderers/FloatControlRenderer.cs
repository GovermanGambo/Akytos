#if AKYTOS_EDITOR

namespace Akytos.Editor.Renderers;

internal class FloatControlRenderer : IGuiControlRenderer<float>
{
    public float DrawControl(string label, float value, object? arguments = null)
    {
        if (AkGui.InputFloat(label, ref value))
        {
            return value;
        }

        return value;
    }

    public object DrawControl(string label, object value, object? arguments = null)
    {
        return DrawControl(label, (float)value);
    }
}

#endif