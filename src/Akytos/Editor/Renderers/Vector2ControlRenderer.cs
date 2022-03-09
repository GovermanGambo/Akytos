using System.Numerics;

namespace Akytos.Editor.Renderers;

public class Vector2ControlRenderer : IGuiControlRenderer<Vector2?>
{
    public Vector2? DrawControl(string label, Vector2? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        Vector2 vector = value.Value;

        if (AkGui.InputVector2(label, ref vector))
        {
            return value;
        }

        return null;
    }

    public object? DrawControl(string label, object? value)
    {
        return DrawControl(label, value as Vector2?);
    }
}