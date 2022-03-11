#if AKYTOS_EDITOR

using System.Numerics;

namespace Akytos.Editor;

internal class EditorOrthographicCamera : IEditorCamera
{
    private float m_zoomLevel;
    private float m_aspectRatio;
    private float m_scaleFactor = 1;
    private Vector2 m_position;

    public EditorOrthographicCamera(int width, int height)
    {
        SetProjection(width, height);
        
        ViewMatrix = Matrix4x4.CreateTranslation(new Vector3(-m_position.X, -m_position.Y, 0.0f));
    }

    public Matrix4x4 ViewMatrix { get; private set; } = Matrix4x4.Identity;
    public Matrix4x4 ProjectionMatrix { get; private set; } = Matrix4x4.Identity;

    public Vector2 Position
    {
        get => m_position;
        set
        {
            m_position = value;
            ViewMatrix = Matrix4x4.CreateTranslation(new Vector3(-m_position.X, -m_position.Y, 0.0f));
        }
    }

    public float ScaleFactor
    {
        get => m_scaleFactor;
        set
        {
            m_zoomLevel *= ScaleFactor;
            m_scaleFactor = value;
            m_zoomLevel /= ScaleFactor;

            CalculateProjectionMatrix();
        }
    }

    public void SetProjection(int width, int height)
    {
        m_aspectRatio = (float) width / height;
        m_zoomLevel = height / ScaleFactor / 2;
        
        CalculateProjectionMatrix();
    }

    private void CalculateProjectionMatrix()
    {
        ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(-m_aspectRatio * m_zoomLevel,
            m_aspectRatio * m_zoomLevel, -m_zoomLevel, m_zoomLevel, 0.01f, 100.0f);
    }
}

#endif