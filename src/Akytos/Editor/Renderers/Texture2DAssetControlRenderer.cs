#if AKYTOS_EDITOR

using System.Runtime.InteropServices;
using Akytos.Assets;
using ImGuiNET;
using Veldrid;

namespace Akytos.Editor.Renderers;

internal class TextureAssetRenderer : IGuiControlRenderer<Asset<Texture>?>
{
    private readonly IAssetManager m_assetManager;

    public TextureAssetRenderer(IAssetManager assetManager)
    {
        m_assetManager = assetManager;
    }

    public unsafe bool DrawControl(string label, ref Asset<Texture>? value, object? arguments = null)
    {
        AkGui.BeginField(label);

        string text = value?.FilePath ?? StringConstants.None;

        bool result = false;

        ImGui.InputText("", ref text, 200, ImGuiInputTextFlags.ReadOnly);
        // TODO: Check payload before trying this?
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload(SystemConstants.DragAndDropIdentifiers.Asset);
            if (payload.NativePtr != null)
            {
                var handle = (GCHandle)payload.Data;

                if (handle.Target is string filePath && Path.GetExtension(filePath) == ".png")
                {
                    if (m_assetManager.Load<Texture>(filePath) is { } asset)
                    {
                        value = asset;
                        result = true;
                    }
                }
            }
            
            
            ImGui.EndDragDropTarget();
        }
        
        ImGui.SameLine();

        ImGui.Button("...");

        AkGui.EndField();

        return result;
    }

    public bool DrawControl(string label, ref object? value, object? arguments = null)
    {
        var textureAsset = value as Asset<Texture>;

        if (DrawControl(label, ref textureAsset, arguments))
        {
            value = textureAsset;
            return true;
        }

        return false;
    }
}

#endif