namespace Akytos.Configuration;

public interface IConfigureGame
{
    void SetInitialWindowSize(int width, int height);
    void SetWindowTitle(string title);
}