#if AKYTOS_EDITOR

using System.Runtime.InteropServices;
using Akytos.Assets;
using Akytos.Graphics;
using ImGuiNET;

namespace Akytos.Editor.Renderers;

internal class Texture2DAssetRenderer : IGuiControlRenderer<Texture2DAsset?>
{
    private readonly AssetManager m_assetManager;

    public Texture2DAssetRenderer(AssetManager assetManager)
    {
        m_assetManager = assetManager;
    }

    public unsafe Texture2DAsset DrawControl(string label, Texture2DAsset? value, object? arguments = null)
    {
        var result = value;
        AkGui.BeginField(label);

        string text = value?.FilePath ?? StringConstants.None;

        ImGui.InputText("", ref text, 200, ImGuiInputTextFlags.ReadOnly);
        // TODO: Check payload before trying this?
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("ASSET");
            if (payload.NativePtr != null)
            {
                var handle = (GCHandle)payload.Data;

                if (handle.Target is string filePath && Path.GetExtension(filePath) == ".png")
                {
                    var asset = m_assetManager.Load<ITexture2D>(filePath) as Texture2DAsset;
                    result = asset ?? result;
                }
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