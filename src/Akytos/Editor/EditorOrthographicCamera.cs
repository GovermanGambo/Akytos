using System.Numerics;
using Akytos.Graphics;

namespace Akytos.Editor;

internal class EditorOrthographicCamera : IEditorCamera
{
    private float m_zoomLevel;
    private float m_aspectRatio;
    // TODO: This should be controlled by zooming in/out
    private int m_scaleFactor = 4;

    public EditorOrthographicCamera(int width, int height)
    {
        SetProjection(width, height);
    }

    public Matrix4x4 ViewMatrix { get; private set; } = Matrix4x4.Identity;
    public Matrix4x4 ProjectionMatrix { get; private set; } = Matrix4x4.Identity;

    public void SetProjection(int width, int height)
    {
        m_aspectRatio = (float) width / height;
        m_zoomLevel = (float)width / m_scaleFactor;
        
        CalculateProjectionMatrix();
    }

    private void CalculateProjectionMatrix()
    {
        ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(-m_aspectRatio * m_zoomLevel,
            m_aspectRatio * m_zoomLevel, -m_zoomLevel, m_zoomLevel, 0.01f, 100.0f);
    }
}