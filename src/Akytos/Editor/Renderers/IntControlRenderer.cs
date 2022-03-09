#if AKYTOS_EDITOR

namespace Akytos.Editor.Renderers;

public class IntControlRenderer : IGuiControlRenderer<int>
{
    public int DrawControl(string label, int value)
    {
        if (AkGui.InputInteger(label, ref value))
        {
            return value;
        }

        return value;
    }

    public object DrawControl(string label, object value)
    {
        return DrawControl(label, (int)value);
    }
}

#endif