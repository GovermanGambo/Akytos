using Akytos.Assets;
using Microsoft.Win32;

namespace Windmill.Services;

public class WindowsFileDialogService : IFileDialogService
{
    public string? OpenFile()
    {
        var fileDialog = new OpenFileDialog();
        fileDialog.InitialDirectory = Asset.AssetPath;
        bool? result = fileDialog.ShowDialog();

        if (result == true)
        {
            return fileDialog.FileName;
        }

        return null;
    }

    public string? SaveFile()
    {
        var fileDialog = new SaveFileDialog();
        fileDialog.InitialDirectory = Asset.AssetPath;
        bool? result = fileDialog.ShowDialog();

        if (result == true)
        {
            return fileDialog.FileName;
        }

        return null;
    }
}