#if AKYTOS_EDITOR

namespace Akytos.Editor;

internal interface IEditorViewport
{
    IEditorCamera Camera { get; }
    int Width { get; }
    int Height { get; }
    void ResizeViewport(int width, int height);
}

#endif