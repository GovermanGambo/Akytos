using Akytos.Assets;
using Akytos.Events;
using ImGuiNET;

namespace Windmill.Panels;

internal class AssetsPanel : IEditorPanel
{
    private static readonly string RootDirectory = Asset.AssetPath;

    public void Dispose()
    {
    }

    public string DisplayName => "Assets";
    public bool IsEnabled { get; set; } = true;
    public string CurrentDirectory { get; private set; } = RootDirectory;
    public void OnDrawGui()
    {
        bool open = IsEnabled;
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
        {
            if (ImGui.Button(".."))
            {
                CurrentDirectory = Path.GetFullPath(Path.Combine(CurrentDirectory, @"../"));
            }
        }
        
        foreach (var directory in directories)
        {
            if (ImGui.Button(directory.Name))
            {
                CurrentDirectory = directory.FullName;
            }
        }
        
        foreach (var file in files)
        {
            ImGui.Text(file.Name);
        }
        
        ImGui.End();
    }

    public void OnEvent(IEvent e)
    {
    }
}