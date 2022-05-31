#if AKYTOS_EDITOR

using Akytos.Diagnostics.Logging;

namespace Akytos.Editor.Renderers;

public class ColorControlRenderer : IGuiControlRenderer<Color>
{
    public bool DrawControl(string label, ref Color value, object? arguments = null)
    {
        return AkGui.InputColor(label, ref value);
    }

    public bool DrawControl(string label, ref object? value, object? arguments = null)
    {
        if (value == null)
        {
            Log.Core.Error("Color value cannot be null!");
            return false;
        }

        var colorValue = (Color)value;

        if (DrawControl(label, ref colorValue, arguments))
        {
            value = colorValue;
            return true;
        }

        return false;
    }
}

#endif