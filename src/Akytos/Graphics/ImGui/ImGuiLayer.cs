using System.Numerics;
using System.Runtime.InteropServices;
using Akytos.Events;
using Akytos.Layers;
using Akytos.Windowing;
using ImGuiNET;
using Veldrid;

namespace Akytos.Graphics;

internal class ImGuiLayer : ILayer
{
    private readonly ImGuiRenderer m_renderer;
    private readonly GameWindow m_window;
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly CommandList m_commandList;

    public ImGuiLayer(GraphicsDevice graphicsDevice, IGameWindow window, CommandList commandList, ImGuiRenderer renderer)
    {
        m_graphicsDevice = graphicsDevice;
        m_commandList = commandList;
        m_renderer = renderer;

        m_window = (GameWindow) window;
        
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        io.Fonts.AddFontFromFileTTF("assets/fonts/open_sans/OpenSans-Regular.ttf", 19.0f);
        io.Fonts.AddFontFromFileTTF("assets/fonts/open_sans/OpenSans-Bold.ttf", 19.0f);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var imguiCtx = ImGui.GetCurrentContext();
            ImGuizmo.SetImGuiContext(imguiCtx);
        }

        SetDarkThemeColors();

        ImGui.LoadIniSettingsFromDisk("imgui.ini");
    }
    
    public void Dispose()
    {
        m_renderer.Dispose();
    }

    public bool IsEnabled { get; set; } = true;
    
    public void OnAttach()
    {
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        var snapshot = m_window.InputSnapshot;
        m_renderer.Update(time, snapshot);
    }

    public void OnRender()
    {
        m_renderer.Render(m_graphicsDevice, m_commandList);
    }

    public void OnEvent(IEvent e)
    {
    }

    public void OnDrawGui()
    {
        
    }

    private void SetDarkThemeColors()
    {
        ImGui.GetStyle().WindowRounding = 0.0f;
        ImGui.GetStyle().TabRounding = 0.0f;

        var colors = ImGui.GetStyle().Colors;
        colors[(int) ImGuiCol.WindowBg] = new Vector4(0f, 0.1f, 0.05f, 1.0f);
            
        colors[(int) ImGuiCol.Header] = new Vector4(0f, 0.4f, 0.2f, 1.0f);
        colors[(int) ImGuiCol.HeaderHovered] = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
        colors[(int) ImGuiCol.HeaderActive] = new Vector4(0f, 0.4f, 0.2f, 1.0f);
        colors[(int) ImGuiCol.HeaderActive | (int) ImGuiCol.HeaderHovered] = new Vector4(0f, 0.4f, 0.2f, 1.0f);
            
        colors[(int) ImGuiCol.Button] = new Vector4(0f, 0.6f, 0.3f, 1.0f);
        colors[(int) ImGuiCol.ButtonHovered] = new Vector4(0f, 0.8f, 0.3f, 1.0f);
        colors[(int) ImGuiCol.ButtonActive] = new Vector4(0f, 0.6f, 0.3f, 1.0f);
        colors[(int) ImGuiCol.Border] = new Vector4(0f, 0.2f, 0.1f, 1.0f);
            
        colors[(int) ImGuiCol.FrameBg] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
        colors[(int) ImGuiCol.FrameBgHovered] = new Vector4(0.3f, 0.305f, 0.31f, 1.0f);
        colors[(int) ImGuiCol.FrameBgActive] = new Vector4(0.15f, 0.155f, 0.151f, 1.0f);
            
        colors[(int) ImGuiCol.Tab] = new Vector4(0f, 0.2f, 0f, 1.0f);
        colors[(int) ImGuiCol.TabHovered] = new Vector4(0f, 0.8f, 0.3f, 1.0f);
        colors[(int) ImGuiCol.TabActive] = new Vector4(0f, 0.6f, 0.3f, 1.0f);
        colors[(int) ImGuiCol.TabUnfocused] = new Vector4(0f, 0.4f, 0.2f, 1.0f);
        colors[(int) ImGuiCol.TabUnfocusedActive] = new Vector4(0f, 0.4f, 0.2f, 1.0f);
            
        colors[(int) ImGuiCol.TitleBg] = new Vector4(0f, 0.2f, 0.1f, 1.0f);
        colors[(int) ImGuiCol.TitleBgActive] = new Vector4(0f, 0.3f, 0.2f, 1.0f);
        colors[(int) ImGuiCol.TitleBgCollapsed] = new Vector4(0.15f, 0.155f, 0.151f, 1.0f);

        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0f, 0.1f, 0f, 1.0f);


    }
}