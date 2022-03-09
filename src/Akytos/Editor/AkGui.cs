#if AKYTOS_EDITOR

using ImGuiNET;

namespace Akytos.Editor;

public static class AkGui
{
    public static bool InputText(string label, ref string value, int maxLength)
    {
        return ImGui.InputText(label, ref value, (uint)maxLength);
    }

    public static bool InputFloat(string label, ref float? value, float step = 0.1f)
    {
        if (value == null) return false;

        float floatValue = value.Value;
        
        return ImGui.InputFloat(label, ref floatValue, step);
    }

    public static bool InputInteger(string label, ref int? value)
    {
        if (value == null) return false;

        int intValue = value.Value;

        return ImGui.InputInt(label, ref intValue);
    }
}

#endif