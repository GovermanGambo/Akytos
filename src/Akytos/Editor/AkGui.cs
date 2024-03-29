#if AKYTOS_EDITOR

using System.Numerics;
using ImGuiNET;

namespace Akytos.Editor;

public static class AkGui
{
    public static void BeginField(string label, float columnWidth = 100)
    {
        ImGui.PushID(label);
        ImGui.Columns(2);
        ImGui.SetColumnWidth(0, columnWidth);
        ImGui.Text(label);
        ImGui.NextColumn();
        
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0.0f, 4.0f));
    }

    public static void EndField()
    {
        ImGui.PopStyleVar();
        ImGui.Columns(1);
        ImGui.PopID();
    }
    
    public static bool InputText(string label, ref string value, int maxLength)
    {
        BeginField(label);   
        
        bool didChange = ImGui.InputText(string.Empty, ref value, (uint) maxLength, ImGuiInputTextFlags.EnterReturnsTrue);
        
        EndField();

        return didChange;
    }

    public static bool InputBool(string label, ref bool value)
    {
        BeginField(label);

        bool didChange = ImGui.Checkbox(string.Empty, ref value);
        
        EndField();

        return didChange;
    }

    public static bool InputFloat(string label, ref float value, float step = 0.1f)
    {
        BeginField(label);
        
        bool didChange = ImGui.InputFloat(string.Empty, ref value, step);
        
        EndField();
        
        return didChange;
    }

    public static bool InputColor(string label, ref Color value)
    {
        BeginField(label);

        var vector4 = (Vector4)value;
        bool didChange = ImGui.ColorEdit4(string.Empty, ref vector4, ImGuiColorEditFlags.NoInputs);
        
        EndField();

        if (didChange)
        {
            value = new Color(vector4);
            return true;
        }

        return false;
    }

    public static bool SliderInt(string label, ref int value, int speed = 1, int min = 0, int max = 10)
    {
        BeginField(label);

        bool didChange = ImGui.DragInt(string.Empty, ref value, speed, min, max);

        EndField();

        return didChange;
    }

    public static bool InputInteger(string label, ref int value)
    {
        BeginField(label);
        
        bool didChange = ImGui.InputInt(string.Empty, ref value, 1, 1);
        
        EndField();

        return didChange;
    }

    public static bool InputVector2(string label, ref Vector2 values, float resetValue = 0.0f,
        float columnWidth = 100)
    {
        var io = ImGui.GetIO();
        var boldFont = io.Fonts.Fonts[0];

        BeginField(label, columnWidth);
        
        bool didChange = false;
        float lineHeight = ImGui.GetFont().FontSize + ImGui.GetStyle().FramePadding.Y * 2.0f;
        var buttonSize = new Vector2(lineHeight + 3.0f, lineHeight);

        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.1f, 0.15f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.9f, 0.2f, 0.2f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.8f, 0.1f, 0.15f, 1.0f));
        ImGui.PushFont(boldFont);
        if (ImGui.Button("X", buttonSize))
        {
            values.X = resetValue;
            didChange = true;
        }

        ImGui.PopFont();
        ImGui.PopStyleColor(3);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(75);
        if (ImGui.DragFloat("##X", ref values.X, 1.0f)) didChange = true;
        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.7f, 0.3f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.8f, 0.3f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.2f, 0.7f, 0.3f, 1.0f));
        ImGui.PushFont(boldFont);
        if (ImGui.Button("Y", buttonSize))
        {
            values.Y = resetValue;
            didChange = true;
        }

        ImGui.PopFont();
        ImGui.PopStyleColor(3);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(75);
        if (ImGui.DragFloat("##Y", ref values.Y, 1.0f)) didChange = true;

        EndField();

        return didChange;
    }
}

#endif