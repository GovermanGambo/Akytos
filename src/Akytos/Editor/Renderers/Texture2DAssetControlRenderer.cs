#if AKYTOS_EDITOR

using System.Runtime.InteropServices;
using Akytos.Assets;
using Akytos.Graphics;
using ImGuiNET;

namespace Akytos.Editor.Renderers;

internal class Texture2DAssetRenderer : IGuiControlRenderer<Texture2DAsset?>
{
    private readonly IAssetManager m_assetManager;

    public Texture2DAssetRenderer(IAssetManager assetManager)
    {
        m_assetManager = assetManager;
    }

    public unsafe bool DrawControl(string label, ref Texture2DAsset? value, object? arguments = null)
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
                    if (m_assetManager.Load<ITexture2D>(filePath) is Texture2DAsset asset)
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
        var textureAsset = value as Texture2DAsset;

        if (DrawControl(label, ref textureAsset, arguments))
        {
            value = textureAsset;
            return true;
        }

        return false;
    }
}

#endif