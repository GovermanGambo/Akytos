#if AKYTOS_EDITOR

namespace Akytos.Editor;

internal class EditorViewport : IEditorViewport
{
    public EditorViewport(int width, int height)
    {
        Width = width;
        Height = height;
        Camera = new EditorOrthographicCamera(Width, Height);
    }

    public IEditorCamera Camera { get; }

    public int Width { get; private set; }
    public int Height { get; private set; }
    
    public void ResizeViewport(int width, int height)
    {
        Width = width;
        Height = height;
        
        Camera.SetProjection(width, height);
    }
}

#endif