using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Akytos;
using Akytos.Assets;
using Akytos.Events;
using ImGuiNET;
using Windmill.ProjectManagement;
using Windmill.Resources;

namespace Windmill.Panels;

internal class AssetsPanel : EditorPanel
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
    
    public PanelSummary Summary { get; }
    public Action<PanelSummary>? Closed { get; set; }

    public void DrawGui()
    {
        bool open = true;
        if (!ImGui.Begin(Summary.DisplayName, ref open))
        {
            ImGui.End();
            Closed?.Invoke(Summary);
        }

        ImGui.End();
    }

    protected override void OnDrawGui()
    {
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
                ImGui.SetDragDropPayload(SystemConstants.DragAndDropIdentifiers.Asset, payload, sizeof(char) * (uint)relativeFilePath.Length);
                handle.Free();
                ImGui.EndDragDropSource();
            }
        }

        ImGui.PopStyleColor();
    }

    protected override PanelSummary ProvideSummary()
    {
        return new PanelSummary("general_assets",LocalizedStrings.Assets, typeof(AssetsPanel));
    }

    private void ProjectManagerOnProjectChanged()
    {
        // Reload the asset directory
        CurrentDirectory = Asset.AssetsDirectory;
    }
}