using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Akytos.Assets;
using Akytos.Events;
using ImGuiNET;
using Windmill.ProjectManagement;
using Windmill.Resources;

namespace Windmill.Panels;

internal class AssetsPanel : IEditorPanel
{
    private readonly IProjectManager m_projectManager;

    public AssetsPanel(IProjectManager projectManager)
    {
        m_projectManager = projectManager;
        m_projectManager.ProjectChanged += ProjectManagerOnProjectChanged;
    }

    public string CurrentDirectory { get; private set; } = Asset.AssetsDirectory;

    public void Dispose()
    {
        m_projectManager.ProjectChanged -= ProjectManagerOnProjectChanged;
    }

    public string DisplayName => LocalizedStrings.Assets;
    public bool IsEnabled { get; set; } = true;

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
                Path.GetFullPath(Asset.AssetsDirectory).TrimEnd('\\'),
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
                var relativeFilePath = Path.GetRelativePath(Asset.AssetsDirectory, file.FullName).Replace("\\", "/");
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

    private void ProjectManagerOnProjectChanged()
    {
        // Reload the asset directory
        CurrentDirectory = Asset.AssetsDirectory;
    }
}