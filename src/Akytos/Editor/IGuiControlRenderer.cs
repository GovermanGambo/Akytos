#if AKYTOS_EDITOR

namespace Akytos.Editor;

public interface IGuiControlRenderer<T> : IGuiControlRenderer
{
    bool DrawControl(string label, ref T value, object? arguments = null);
}

public interface IGuiControlRenderer
{
    bool DrawControl(string label, ref object? value, object? arguments = null);
}

#endif