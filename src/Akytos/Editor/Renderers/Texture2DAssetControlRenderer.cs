using Akytos.Assets;
using ImGuiNET;

namespace Akytos.Editor.Renderers;

public class Texture2DAssetRenderer : IGuiControlRenderer<Texture2DAsset>
{
    public Texture2DAsset DrawControl(string label, Texture2DAsset value, object? arguments = null)
    {
        // TODO: Allow for drag/drop, and add button for selecting asset
        AkGui.BeginField(label);

        string text = value.FilePath;

        ImGui.InputText("", ref text, 200, ImGuiInputTextFlags.ReadOnly);
        
        ImGui.SameLine();

        ImGui.Button("...");

        AkGui.EndField();

        return value;
    }

    public object DrawControl(string label, object value, object? arguments = null)
    {
        return DrawControl(label, (Texture2DAsset)value, arguments);
    }
}