using System.Numerics;

namespace Akytos.Graphics;

public interface ICamera
{
    Matrix4x4 ViewMatrix { get; }
    Matrix4x4 ProjectionMatrix { get; }
}