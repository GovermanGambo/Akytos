using System.Numerics;

namespace Akytos.Editor.Renderers;

public class Vector2ControlRenderer : IGuiControlRenderer<Vector2>
{
    public Vector2 DrawControl(string label, Vector2 value, object? arguments = null)
    {
        if (AkGui.InputVector2(label, ref value))
        {
            return value;
        }

        return value;
    }

    public object DrawControl(string label, object value, object? arguments = null)
    {
        return DrawControl(label, (Vector2)value);
    }
}