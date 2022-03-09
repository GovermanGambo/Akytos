#if AKYTOS_EDITOR

using System.Numerics;
using ImGuiNET;

namespace Akytos.Editor;

public static class AkGui
{
    public static bool InputText(string label, ref string value, int maxLength)
    {
        return ImGui.InputText(label, ref value, (uint) maxLength);
    }

    public static bool InputFloat(string label, ref float value, float step = 0.1f)
    {
        return ImGui.InputFloat(label, ref value, step);
    }

    public static bool InputInteger(string label, ref int value)
    {
        return ImGui.InputInt(label, ref value);
    }

    public static bool InputVector2(string label, ref Vector2 values, float resetValue = 0.0f,
        float columnWidth = 100)
    {
        var io = ImGui.GetIO();
        var boldFont = io.Fonts.Fonts[0];

        ImGui.PushID(label);

        bool changed = false;
        ImGui.Columns(2);

        ImGui.SetColumnWidth(0, columnWidth);
        ImGui.Text(label);
        ImGui.NextColumn();

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
        float lineHeight = ImGui.GetFont().FontSize + ImGui.GetStyle().FramePadding.Y * 2.0f;
        var buttonSize = new Vector2(lineHeight + 3.0f, lineHeight);

        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.1f, 0.15f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.9f, 0.2f, 0.2f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.8f, 0.1f, 0.15f, 1.0f));
        ImGui.PushFont(boldFont);
        if (ImGui.Button("X", buttonSize))
        {
            values.X = resetValue;
            changed = true;
        }

        ImGui.PopFont();
        ImGui.PopStyleColor(3);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(75);
        if (ImGui.DragFloat("##X", ref values.X, 0.1f)) changed = true;
        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.7f, 0.3f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.8f, 0.3f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.2f, 0.7f, 0.3f, 1.0f));
        ImGui.PushFont(boldFont);
        if (ImGui.Button("Y", buttonSize))
        {
            values.Y = resetValue;
            changed = true;
        }

        ImGui.PopFont();
        ImGui.PopStyleColor(3);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(75);
        if (ImGui.DragFloat("##Y", ref values.Y, 0.1f)) changed = true;
        ImGui.PopStyleVar();

        ImGui.Columns(1);

        ImGui.PopID();

        return changed;
    }
}

#endif