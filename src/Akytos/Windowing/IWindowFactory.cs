namespace Akytos.Windowing;

internal interface IWindowFactory
{
    IGameWindow Create(WindowProperties properties);
}