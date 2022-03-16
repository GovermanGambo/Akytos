using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Akytos.Assets;
using Akytos.Events;
using ImGuiNET;

namespace Windmill.Panels;

internal class AssetsPanel : IEditorPanel
{
    private static readonly string RootDirectory = Asset.AssetsDirectory;
    public string CurrentDirectory { get; private set; } = RootDirectory;

    public void Dispose()
    {
    }

    public string DisplayName => "Assets";
    public bool IsEnabled { get; set; } = true;

    public bool HideInMenu { get; }

    public void OnDrawGui()
    {
        var open = IsEnabled;
        if (!ImGui.Begin(DisplayName, ref open))
        {
            IsEnabled = false;
            ImGui.End();
            return;
        }

        var directoryInfo = new DirectoryInfo(CurrentDirectory);

        var directories = directoryInfo.GetDirectories().ToArray();
        var files = directoryInfo.GetFiles().ToArray();

        if (string.Compare(
                Path.GetFullPath(CurrentDirectory).TrimEnd('\\'),
                Path.GetFullPath(RootDirectory).TrimEnd('\\'),
                StringComparison.InvariantCultureIgnoreCase) != 0)
            if (ImGui.Button(".."))
                CurrentDirectory = Path.GetFullPath(Path.Combine(CurrentDirectory, @"../"));

        foreach (var directory in directories)
            if (ImGui.Button(directory.Name))
                CurrentDirectory = directory.FullName;
        ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
        foreach (var file in files)
        {
            ImGui.Selectable(file.Name);
            if (ImGui.BeginDragDropSource())
            {
                string relativeFilePath = Path.GetRelativePath(RootDirectory, file.FullName).Replace("\\", "/");
                var handle = GCHandle.Alloc(relativeFilePath);
                var payload = (IntPtr)handle;
                ImGui.SetDragDropPayload("ASSET", payload, sizeof(char) * (uint)relativeFilePath.Length);
                handle.Free();
                ImGui.EndDragDropSource();
            }
        }

        ImGui.PopStyleColor();

        ImGui.End();
    }

    public void OnEvent(IEvent e)
    {
    }
}