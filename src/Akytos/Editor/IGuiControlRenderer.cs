#if AKYTOS_EDITOR

namespace Akytos.Editor;

public interface IGuiControlRenderer<T> : IGuiControlRenderer
{
    T DrawControl(string label, T value, object? arguments = null);
}

public interface IGuiControlRenderer
{
    object DrawControl(string label, object value, object? arguments = null);
}

#endif