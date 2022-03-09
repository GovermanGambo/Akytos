#if AKYTOS_EDITOR

namespace Akytos.Editor.Renderers;

public class IntControlRenderer : IGuiControlRenderer<int?>
{
    public int? DrawControl(string label, int? value)
    {
        if (AkGui.InputInteger(label, ref value))
        {
            return value;
        }

        return null;
    }

    public object? DrawControl(string label, object? value)
    {
        return DrawControl(label, value as int?);
    }
}

#endif