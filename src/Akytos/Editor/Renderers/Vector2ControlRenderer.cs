using System.Numerics;
using Akytos.Diagnostics.Logging;

namespace Akytos.Editor.Renderers;

public class Vector2ControlRenderer : IGuiControlRenderer<Vector2>
{
    public bool DrawControl(string label, ref Vector2 value, object? arguments = null)
    {
        return AkGui.InputVector2(label, ref value);
    }

    public bool DrawControl(string label, ref object? value, object? arguments = null)
    {
        if (value == null)
        {
            Log.Core.Error("Vector2 values cannot be null!");
            return false;
        }

        var vector2 = (Vector2) value;

        if (DrawControl(label, ref vector2, arguments))
        {
            value = vector2;
            return true;
        }

        return false;
    }
}