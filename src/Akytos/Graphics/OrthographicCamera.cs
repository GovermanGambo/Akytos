using System.Numerics;

namespace Akytos.Graphics;

public class OrthographicCamera : ICamera
{
    private float m_zoomLevel;
    private float m_aspectRatio;
    private Vector2 m_position;

    public OrthographicCamera(int width, int height)
    {
        SetProjection(width, height);
        
        ViewMatrix = Matrix4x4.CreateTranslation(new Vector3(-m_position.X, -m_position.Y, 0.0f));
    }

    public Matrix4x4 ViewMatrix { get; private set; }
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

    public void SetProjection(int width, int height)
    {
        m_aspectRatio = (float) width / height;
        m_zoomLevel = (float)height / 2;
        
        CalculateProjectionMatrix();
    }

    private void CalculateProjectionMatrix()
    {
        ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(-m_aspectRatio * m_zoomLevel,
            m_aspectRatio * m_zoomLevel, -m_zoomLevel, m_zoomLevel, 0.01f, 100.0f);
    }
}