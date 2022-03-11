#if AKYTOS_EDITOR

using System.Runtime.InteropServices;
using Akytos.Assets;
using Akytos.Graphics;
using ImGuiNET;

namespace Akytos.Editor.Renderers;

internal class Texture2DAssetRenderer : IGuiControlRenderer<Texture2DAsset>
{
    private readonly AssetManager m_assetManager;

    public Texture2DAssetRenderer(AssetManager assetManager)
    {
        m_assetManager = assetManager;
    }

    public Texture2DAsset DrawControl(string label, Texture2DAsset value, object? arguments = null)
    {
        var result = value;
        // TODO: Allow for drag/drop, and add button for selecting asset
        AkGui.BeginField(label);

        string text = value.FilePath;

        ImGui.InputText("", ref text, 200, ImGuiInputTextFlags.ReadOnly);
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.GetDragDropPayload();
            var handle = (GCHandle)payload.Data;

            if (handle.Target is string filePath)
            {
                var asset = m_assetManager.Load<ITexture2D>(filePath) as Texture2DAsset;
                result = asset ?? result;
            }
            
            ImGui.EndDragDropTarget();
        }
        
        ImGui.SameLine();

        ImGui.Button("...");

        AkGui.EndField();

        return result;
    }

    public object DrawControl(string label, object value, object? arguments = null)
    {
        return DrawControl(label, (Texture2DAsset)value, arguments);
    }
}

#endif