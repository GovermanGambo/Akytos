#if AKYTOS_EDITOR

namespace Akytos.Editor.Renderers;

internal class TextControlRenderer : IGuiControlRenderer<string>
{
    public string DrawControl(string label, string value, object? arguments = null)
    {
        if (AkGui.InputText(label, ref value, 50))
        {
            return value;
        }

        return value;
    }

    public object DrawControl(string label, object value, object? arguments = null)
    {
        return DrawControl(label, (string)value);
    }
}

#endif