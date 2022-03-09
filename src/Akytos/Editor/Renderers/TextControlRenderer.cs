#if AKYTOS_EDITOR

namespace Akytos.Editor.Renderers;

internal class TextControlRenderer : IGuiControlRenderer<string>
{
    public string? DrawControl(string label, string? value)
    {
        if (AkGui.InputText(label, ref value, 50))
        {
            return value;
        }

        return null;
    }

    public object? DrawControl(string label, object? value)
    {
        return DrawControl(label, value as string);
    }
}

#endif