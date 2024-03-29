using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Akytos;
using Akytos.Editor;
using Akytos.SceneSystems;
using ImGuiNET;
using Math = System.Math;

namespace Windmill.Services;

internal class GizmoService
{
    private const GizmoMode DefaultGizmoMode = GizmoMode.Translate;

    public bool IsUsing => ImGuizmo.IsUsing();
    public bool IsSnapping { get; set; }

    public GizmoMode GizmoMode { get; set; } = DefaultGizmoMode;

    public void DrawGizmos(IEditorCamera camera, Node2D node2D)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new NotSupportedException("Gizmos are only supported on Windows!");
        }
        
        if (GizmoMode == GizmoMode.None) return;

        ImGuizmo.SetOrthographic(true);
        ImGuizmo.SetDrawlist();

        float windowWidth = ImGui.GetWindowWidth();
        float windowHeight = ImGui.GetWindowHeight();
        ImGuizmo.SetRect(ImGui.GetWindowPos().X, ImGui.GetWindowPos().Y, windowWidth, windowHeight);

        var cameraProjection = camera.ProjectionMatrix;
        var cameraView = camera.ViewMatrix;

        var transform = node2D.GetTransform();
        var transformCopy = transform;
        var deltaMatrix = Matrix4x4.Identity;

        float snapValue;

        if (IsSnapping)
        {
            snapValue = 32f;

            if (GizmoMode == GizmoMode.Rotate) snapValue = 22.5f;
        }
        else
        {
            snapValue = 1.0f;
        }
        
        float[] snapValues = {snapValue, snapValue, snapValue};

        // TODO: Something is making this method spit out a NaN transform when using axis handles
        ImGuizmo.Manipulate(ref cameraView.M11, ref cameraProjection.M11, (OPERATION) GizmoMode - 1, MODE.WORLD,
            ref transformCopy.M11, ref deltaMatrix.M11, ref snapValues[0]);

        if (ImGuizmo.IsUsing())
        {
            Matrix4x4.Decompose(transformCopy, out var scale, out var rotation, out var translation);
            float rotationZ = ToEulerAngles(rotation).Z;

            node2D.GlobalPosition = new Vector2(translation.X, translation.Y);
            node2D.GlobalRotation = rotationZ;
            node2D.GlobalScale = new Vector2(scale.X, scale.Y);
        }
    }

    private static Vector3 ToEulerAngles(Quaternion q)
    {
        Vector3 angles = new();

        // roll / x
        double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
        double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        angles.X = (float) Math.Atan2(sinr_cosp, cosr_cosp);

        // pitch / y
        double sinp = 2 * (q.W * q.Y - q.Z * q.X);
        if (Math.Abs(sinp) >= 1)
            angles.Y = (float) Math.CopySign(Math.PI / 2, sinp);
        else
            angles.Y = (float) Math.Asin(sinp);

        // yaw / z
        double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
        double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        angles.Z = (float) Math.Atan2(siny_cosp, cosy_cosp);

        return angles;
    }
}