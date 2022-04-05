using System.Numerics;
using Silk.NET.Input;

namespace Akytos;

public static class Input
{
    private static IInputContext? s_inputContext;

    public static Vector2 MousePosition
    {
        get
        {
            if (s_inputContext is null) throw new NotInitializedException(nameof(Input));

            return s_inputContext.Mice[0].Position;
        }
    }

    internal static void Initialize(IInputContext inputContext)
    {
        s_inputContext = inputContext;
    }

    public static bool GetKeyPressed(KeyCode keyCode)
    {
        if (s_inputContext is null) throw new NotInitializedException(nameof(Input));

        return s_inputContext.Keyboards.Any(k => k.IsKeyPressed((Key) keyCode));
    }

    public static bool GetMouseButtonPressed(MouseButton mouseButton)
    {
        if (s_inputContext is null) throw new NotInitializedException(nameof(Input));

        return s_inputContext.Mice.Any(m => m.IsButtonPressed((Silk.NET.Input.MouseButton) mouseButton));
    }
}