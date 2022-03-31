namespace Akytos;

/// <summary>
///     Used to configure the Akytos application
/// </summary>
internal class AppConfigurator
{
    public bool EnableImGui { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Width { get; set; } = 960;
    public int Height { get; set; } = 640;
}