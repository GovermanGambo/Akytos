using Akytos.Graphics;

namespace Akytos.Editor;

internal interface IEditorCamera : ICamera
{
    public void SetProjection(int width, int height);
}