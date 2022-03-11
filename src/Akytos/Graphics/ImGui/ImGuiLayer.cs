using System.Numerics;
using Akytos.Events;
using Akytos.Layers;
using Akytos.Windowing;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Akytos.Graphics;

internal class ImGuiLayer : ILayer
{
    private readonly IImGuiController m_controller;

    public ImGuiLayer(IGameWindow window)
    {
        var nativeWindow = window.GetNativeWindow() as IWindow;

        if (nativeWindow == null)
        {
            throw new ArgumentOutOfRangeException();
        }
        
        var config = new ImGuiFontConfig("assets/fonts/open_sans/OpenSans-Regular.ttf", 19);
        m_controller = new OpenGLImGuiController(nativeWindow.CreateOpenGL(),
            nativeWindow,
            nativeWindow.CreateInput(),
            config);
        
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        io.Fonts.AddFontFromFileTTF("assets/fonts/open_sans/OpenSans-Regular.ttf", 19.0f);
        io.Fonts.AddFontFromFileTTF("assets/fonts/open_sans/OpenSans-Bold.ttf", 19.0f);

        var imguiCtx = ImGui.GetCurrentContext();
        ImGuizmo.SetImGuiContext(imguiCtx);

        SetDarkThemeColors();

        ImGui.LoadIniSettingsFromDisk("imgui.ini");
    }
    
    public void Dispose()
    {
        m_controller.Dispose();
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
        m_controller.Update(time);
    }

    public void OnRender()
    {
        m_controller.Render();
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