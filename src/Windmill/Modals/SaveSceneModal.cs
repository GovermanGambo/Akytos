using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Akytos;
using Akytos.Assets;
using Akytos.Events;
using ImGuiNET;
using Windmill.Services;

namespace Windmill.Modals;

internal class SaveSceneModal : IModal
{
    private const ImGuiWindowFlags ModalFlags = ImGuiWindowFlags.NoResize;

    private static string RootDirectory => Asset.AssetsDirectory;

    private readonly SceneEditorContext m_editorContext;

    private string m_currentSubDirectory = "";
    private string m_filename = "";
    private bool m_isOpen;
    private bool m_shouldOpen;

    public SaveSceneModal(SceneEditorContext editorContext)
    {
        m_editorContext = editorContext;
        CurrentDirectory = RootDirectory;
    }

    private string CurrentDirectory { get; set; }

    public void Dispose()
    {
    }

    public string Name => "Save Scene";

    public bool IsOpen
    {
        get => m_isOpen;
        private set
        {
            m_isOpen = value;

            if (!value) Closing?.Invoke();
        }
    }

    public event Action? Closing;

    public void Open()
    {
        m_shouldOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    public void OnDrawGui()
    {
        if (m_shouldOpen)
        {
            ImGui.OpenPopup(Name);
            IsOpen = true;
            m_shouldOpen = false;
        }

        var open = IsOpen;

        if (open) ImGui.SetNextWindowSize(ImGui.GetWindowSize() / 2f);

        if (!ImGui.BeginPopupModal(Name, ref open))
        {
            IsOpen = false;
            return;
        }

        var displayFilePath = GetDisplayFilePath();
        if (ImGui.InputText("Path", ref displayFilePath, 100, ImGuiInputTextFlags.ReadOnly))
            m_currentSubDirectory = GetPathWithoutPrefix(displayFilePath);

        var height = ImGui.GetFrameHeight() - ImGui.GetTextLineHeightWithSpacing() * 2f - 10f;

        if (ImGui.BeginChildFrame(ImGui.GetID("frame"),
                new Vector2(ImGui.GetWindowWidth(), height)))
        {
            DrawFolderContent();

            ImGui.EndChildFrame();
        }

        ImGui.Columns(2);

        ImGui.SetColumnWidth(0, ImGui.GetWindowWidth() - 200f);

        ImGui.SetNextItemWidth(-1);

        var filename = m_filename;
        if (ImGui.InputText("##filename", ref filename, 100)) m_filename = filename;

        ImGui.NextColumn();

        var canSave = !(m_filename == string.Empty || m_filename.IndexOfAny(Path.GetInvalidFileNameChars()) != -1);
        if (ImGui.Button("Save") && canSave)
        {
            if (!m_filename.EndsWith(SystemConstants.FileSystem.SceneFileExtension)) m_filename += SystemConstants.FileSystem.SceneFileExtension;

            m_editorContext.SaveSceneAs(Path.Combine(CurrentDirectory, m_filename));
            IsOpen = false;
        }

        ImGui.SameLine();

        if (ImGui.Button("Cancel")) IsOpen = false;

        ImGui.Columns(1);

        ImGui.End();
    }

    public void OnEvent(IEvent e)
    {
    }

    private void DrawFolderContent()
    {
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
            {
                CurrentDirectory = directory.FullName;
                m_currentSubDirectory = Path.GetRelativePath(RootDirectory, directory.FullName).Replace("\\", "/");
            }

        ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
        foreach (var file in files)
            if (ImGui.Selectable(file.Name))
                m_filename = file.Name;

        ImGui.PopStyleColor();
    }

    private string GetDisplayFilePath()
    {
        return $"assets://{m_currentSubDirectory}";
    }

    private string GetPathWithoutPrefix(string path)
    {
        var startIndex = path.IndexOf("://", StringComparison.Ordinal) + 3;

        startIndex = startIndex <= path.Length - 1 ? startIndex : path.Length;

        return startIndex != -1 ? path[startIndex..] : path;
    }
}