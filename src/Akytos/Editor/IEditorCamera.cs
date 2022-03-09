#if AKYTOS_EDITOR

using Akytos.Graphics;

namespace Akytos.Editor;

internal interface IEditorCamera : ICamera
{
    void SetProjection(int width, int height);
}

#endif