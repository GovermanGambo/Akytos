using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Akytos.Assets;
using Akytos.Events;
using ImGuiNET;
using Windmill.Services;

namespace Windmill.Modals;

internal class LoadSceneModal : IModal
{
    private const ImGuiWindowFlags ModalFlags = ImGuiWindowFlags.NoResize;

    private static readonly string RootDirectory = Asset.AssetsDirectory;

    private readonly SceneEditorContext m_editorContext;

    private string m_currentDirectory;
    private string m_currentSubDirectory = "";
    private string m_filename = "";
    private bool m_isOpen;
    private bool m_shouldOpen;

    public LoadSceneModal(SceneEditorContext editorContext)
    {
        m_editorContext = editorContext;
        m_currentDirectory = RootDirectory;
    }

    public void Dispose()
    {
    }

    public string Name => "Load Scene";

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

    public void Show()
    {
        m_shouldOpen = true;
    }

    public void Hide()
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

        bool open = IsOpen;

        if (open) ImGui.SetNextWindowSize(ImGui.GetWindowSize() / 2f);

        if (!ImGui.BeginPopupModal(Name, ref open, ModalFlags))
        {
            IsOpen = false;
            return;
        }

        string displayFilePath = GetDisplayFilePath();
        if (ImGui.InputText("Path", ref displayFilePath, 100, ImGuiInputTextFlags.ReadOnly))
            m_currentSubDirectory = GetPathWithoutPrefix(displayFilePath);

        float height = ImGui.GetFrameHeight() - ImGui.GetTextLineHeightWithSpacing() * 2f - 10f;

        if (ImGui.BeginChildFrame(ImGui.GetID("frame"),
                new Vector2(ImGui.GetWindowWidth(), height)))
        {
            DrawFolderContent();

            ImGui.EndChildFrame();
        }

        ImGui.Columns(2);

        ImGui.SetColumnWidth(0, ImGui.GetWindowWidth() - 200f);

        ImGui.SetNextItemWidth(-1);

        string filename = m_filename;
        if (ImGui.InputText("##filename", ref filename, 100)) m_filename = filename;

        ImGui.NextColumn();

        bool canLoad = !(m_filename == string.Empty || m_filename.IndexOfAny(Path.GetInvalidFileNameChars()) != -1);
        if (ImGui.Button("Load") && canLoad)
        {
            if (!m_filename.EndsWith(".ascn")) m_filename += ".ascn";

            m_editorContext.LoadScene(Path.Combine(m_currentDirectory, m_filename));
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
        var directoryInfo = new DirectoryInfo(m_currentDirectory);

        var directories = directoryInfo.GetDirectories().ToArray();
        var files = directoryInfo.GetFiles().ToArray();

        if (string.Compare(
                Path.GetFullPath(m_currentDirectory).TrimEnd('\\'),
                Path.GetFullPath(RootDirectory).TrimEnd('\\'),
                StringComparison.InvariantCultureIgnoreCase) != 0)
            if (ImGui.Button(".."))
                m_currentDirectory = Path.GetFullPath(Path.Combine(m_currentDirectory, @"../"));

        foreach (var directory in directories)
            if (ImGui.Button(directory.Name))
            {
                m_currentDirectory = directory.FullName;
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
        int startIndex = path.IndexOf("://", StringComparison.Ordinal) + 3;

        startIndex = startIndex <= path.Length - 1 ? startIndex : path.Length;

        return startIndex != -1 ? path[startIndex..] : path;
    }
}