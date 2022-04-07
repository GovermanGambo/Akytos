using Akytos.Analytics;

#if AKYTOS_EDITOR

namespace Akytos.Editor.Renderers;

public class IntControlRenderer : IGuiControlRenderer<int>
{
    public bool DrawControl(string label, ref int value, object? arguments = null)
    {
        if (arguments is IntSliderAttribute attribute)
        {
            return AkGui.SliderInt(label, ref value, 1, attribute.Min, attribute.Max);
        }

        return AkGui.InputInteger(label, ref value);
    }

    public bool DrawControl(string label, ref object? value, object? arguments = null)
    {
        if (value == null)
        {
            Log.Core.Error("Integer values cannot be null!");
            return false;
        }

        int intValue = (int) value;

        if (DrawControl(label, ref intValue, arguments))
        {
            value = intValue;
            return true;
        }

        return false;
    }
}

#endif