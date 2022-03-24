#if AKYTOS_EDITOR

namespace Akytos.Editor.Renderers;

internal class TextControlRenderer : IGuiControlRenderer<string>
{
    public bool DrawControl(string label, ref string value, object? arguments = null)
    {
        return AkGui.InputText(label, ref value, 50);
    }

    public bool DrawControl(string label, ref object? value, object? arguments = null)
    {
        if (value == null)
        {
            Debug.LogError("String values cannot be null!");
            return false;
        }

        string stringValue = (string) value;

        if (DrawControl(label, ref stringValue, arguments))
        {
            value = stringValue;
            return true;
        }

        return false;
    }
}

#endif