#if AKYTOS_EDITOR

namespace Akytos.Editor.Renderers;

public class IntControlRenderer : IGuiControlRenderer<int>
{
    public int DrawControl(string label, int value, object? arguments = null)
    {
        if (arguments is IntSliderAttribute attribute)
        {
            if (AkGui.SliderInt(label, ref value, 1, attribute.Min, attribute.Max))
            {
                return value;
            }
        }
        else if (AkGui.InputInteger(label, ref value))
        {
            return value;
        }

        return value;
    }

    public object DrawControl(string label, object value, object? arguments = null)
    {
        return DrawControl(label, (int)value, arguments);
    }
}

#endif