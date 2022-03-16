using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Akytos;
using Akytos.Assets;
using ImGuiNET;

namespace Windmill.Modals;

public class FileDialogModal : IModal
{
    private static readonly string RootDirectory = Asset.AssetPath;

    private string m_currentSubDirectory = "./";
    private string m_fileName = "";
    private TaskCompletionSource<bool> m_taskCompletionSource = null!;

    public FileDialogModal(string name)
    {
        Name = name;
        CurrentDirectory = RootDirectory;
    }

    public void Dispose()
    {

    }

    public string Name { get; }

    public bool IsOpen { get; set; }

    private string CurrentDirectory { get; set; }

    public void OnAppearing()
    {
    }

    public Task<bool> ShowDialog()
    {
        m_taskCompletionSource = new TaskCompletionSource<bool>();

        IsOpen = true;
        ImGui.OpenPopup("Save Scene");

        return m_taskCompletionSource.Task;
    }

    public void OnDrawGui(DeltaTime deltaTime)
    {
        bool open = IsOpen;
        if (!ImGui.BeginPopupModal("Save Scene", ref open))
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

        string fileName = m_fileName;
        if (ImGui.InputText("", ref fileName, 100))
        {
            m_fileName = fileName;
        }
        
        ImGui.NextColumn();

        if (ImGui.Button("Save"))
        {
            m_taskCompletionSource.SetResult(true);
            IsOpen = false;
        }
        
        ImGui.NextColumn();

        if (ImGui.Button("Cancel"))
        {
            m_taskCompletionSource.SetResult(false);
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