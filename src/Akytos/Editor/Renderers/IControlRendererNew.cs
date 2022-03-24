namespace Akytos.Editor.Renderers;

public interface IControlRendererNew
{
    bool DrawControl(string label, ref object? value, object? arguments = null);
}

public interface IControlRendererNew<T> : IControlRendererNew
{
    new bool DrawControl(string label, ref T? value, object? arguments = null);
}