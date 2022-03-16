namespace Windmill.Services;

public interface IFileDialogService
{
    string? OpenFile();
    string? SaveFile();
}