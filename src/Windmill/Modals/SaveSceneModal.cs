using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Akytos;
using Akytos.Assets;
using ImGuiNET;
using Windmill.Services;

namespace Windmill.Modals;

internal class SaveSceneModal : IModal
{
    private static readonly string RootDirectory = Asset.AssetPath;

    private readonly SceneEditorContext m_editorContext;

    private string m_currentSubDirectory = "./";
    private string m_filename = "";

    public SaveSceneModal(SceneEditorContext editorContext)
    {
        m_editorContext = editorContext;
        CurrentDirectory = RootDirectory;
    }

    public void Dispose()
    {

    }

    public string Name => "Save Scene";

    public bool IsOpen { get; set; }

    private string CurrentDirectory { get; set; }

    public void OnAppearing()
    {
    }

    public void OnDrawGui()
    {
        bool open = IsOpen;
        if (!ImGui.BeginPopupModal(Name, ref open))
        {
            return;
        }

        float windowWidth = ImGui.GetWindowWidth();
        float textWidth = ImGui.CalcTextSize(Name).X;
        ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
        ImGui.Text(Name);

        ImGui.Separator();

        string displayFilePath = GetDisplayFilePath();
        if (ImGui.InputText("", ref displayFilePath, 100))
        {
            m_currentSubDirectory = GetPathWithoutPrefix(displayFilePath);
        }
        
        ImGui.Separator();
        
        DrawFolderContent();
        
        ImGui.Separator();

        ImGui.Columns(3);
        ImGui.SetColumnWidth(1, 100);
        ImGui.SetColumnWidth(2, 100);

        string filename = m_filename;
        if (ImGui.InputText("", ref filename, 100))
        {
            m_filename = filename;
        }
        
        ImGui.NextColumn();

        bool canSave = !(m_filename == string.Empty || m_filename.IndexOfAny(Path.GetInvalidFileNameChars()) != -1);

        if (ImGui.Button("Save") && canSave)
        {
            if (!m_filename.EndsWith(".ascn"))
            {
                m_filename += ".ascn";
            }
            
            m_editorContext.SaveSceneAs(Path.Combine(CurrentDirectory, m_filename));
            IsOpen = false;
        }
        
        ImGui.NextColumn();

        if (ImGui.Button("Cancel"))
        {
            IsOpen = false;
        }
        
        ImGui.Columns(1);

        ImGui.End();
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
        {
            if (ImGui.Button(directory.Name))
            {
                CurrentDirectory = directory.FullName;
                m_currentSubDirectory = Path.GetRelativePath(RootDirectory, directory.FullName).Replace("\\", "/");
            }
        }
            
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
    }

    private string GetDisplayFilePath()
    {
        return $"{RootDirectory}://{m_currentSubDirectory}";
    }

    private string GetPathWithoutPrefix(string path)
    {
        int startIndex = path.IndexOf("://", StringComparison.Ordinal) + 3;

        return startIndex != -1 ? path[startIndex..] : path;
    }

}