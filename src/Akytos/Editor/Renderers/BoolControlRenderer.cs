namespace Akytos.Editor.Renderers;

public class BoolControlRenderer : IControlRendererNew<bool>
{
    public bool DrawControl(string label, ref bool value, object? arguments = null)
    {
        return AkGui.InputBool(label, ref value);
    }

    public bool DrawControl(string label, ref object? value, object? arguments = null)
    {
        if (value == null)
        {
            throw new ArgumentException();
        }
        
        bool boolValue = (bool) value;

        if (DrawControl(label, ref boolValue, arguments))
        {
            value = boolValue;
            return true;
        }

        return false;
    }
}