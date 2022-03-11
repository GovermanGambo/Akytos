using System.Numerics;

#if AKYTOS_EDITOR

namespace Akytos.Editor;

internal interface IEditorViewport
{
    IEditorCamera Camera { get; }
    int Width { get; }
    int Height { get; }
    Vector2 Size => new(Width, Height);
    void ResizeViewport(int width, int height);
}

#endif