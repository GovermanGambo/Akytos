#if AKYTOS_EDITOR

using System.Numerics;
using Akytos.Graphics;

namespace Akytos.Editor;

internal interface IEditorCamera : ICamera
{
    Vector2 Position { get; set; }
    float ScaleFactor { get; set; }
    void SetProjection(int width, int height);
}

#endif