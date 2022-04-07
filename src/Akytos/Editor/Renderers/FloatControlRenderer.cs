using Akytos.Analytics;

#if AKYTOS_EDITOR

namespace Akytos.Editor.Renderers;

internal class FloatControlRenderer : IGuiControlRenderer<float>
{
    public bool DrawControl(string label, ref float value, object? arguments = null)
    {
        return AkGui.InputFloat(label, ref value);
    }

    public bool DrawControl(string label, ref object? value, object? arguments = null)
    {
        if (value == null)
        {
            Log.Core.Error("Float values cannot be null!");
            return false;
        }

        float floatValue = (float) value;

        if (DrawControl(label, ref floatValue, arguments))
        {
            value = floatValue;
            return true;
        }

        return false;
    }
}

#endif