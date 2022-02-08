namespace Akytos.Windowing;

public interface IWindowFactory
{
    IGameWindow Create(WindowProperties properties);
}