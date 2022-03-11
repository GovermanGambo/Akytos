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
        
        var config = new ImGuiFontConfig("assets/fonts/open_sans/OpenSans-Regular.ttf", 18);
        m_controller = new OpenGLImGuiController(nativeWindow.CreateOpenGL(),
            nativeWindow,
            nativeWindow.CreateInput(),
            config);
        
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        io.Fonts.AddFontFromFileTTF("assets/fonts/open_sans/OpenSans-Regular.ttf", 18.0f);
        io.Fonts.AddFontFromFileTTF("assets/fonts/open_sans/OpenSans-Bold.ttf", 18.0f);

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
        var colors = ImGui.GetStyle().Colors;
        ImGui.GetStyle().WindowRounding = 0.0f;
        colors[(int) ImGuiCol.WindowBg] = new Vector4(0.1f, 0.105f, 0.11f, 1.0f);
            
        colors[(int) ImGuiCol.Header] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
        colors[(int) ImGuiCol.HeaderHovered] = new Vector4(0.3f, 0.305f, 0.31f, 1.0f);
        colors[(int) ImGuiCol.HeaderActive] = new Vector4(0.15f, 0.155f, 0.151f, 1.0f);
            
        colors[(int) ImGuiCol.Button] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
        colors[(int) ImGuiCol.ButtonHovered] = new Vector4(0.3f, 0.305f, 0.31f, 1.0f);
        colors[(int) ImGuiCol.ButtonActive] = new Vector4(0.15f, 0.155f, 0.151f, 1.0f);
            
        colors[(int) ImGuiCol.FrameBg] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
        colors[(int) ImGuiCol.FrameBgHovered] = new Vector4(0.3f, 0.305f, 0.31f, 1.0f);
        colors[(int) ImGuiCol.FrameBgActive] = new Vector4(0.15f, 0.155f, 0.151f, 1.0f);
            
        colors[(int) ImGuiCol.Tab] = new Vector4(0.15f, 0.155f, 0.151f, 1.0f);
        colors[(int) ImGuiCol.TabHovered] = new Vector4(0.38f, 0.3805f, 0.381f, 1.0f);
        colors[(int) ImGuiCol.TabActive] = new Vector4(0.28f, 0.2805f, 0.281f, 1.0f);
        colors[(int) ImGuiCol.TabUnfocused] = new Vector4(0.15f, 0.155f, 0.151f, 1.0f);
        colors[(int) ImGuiCol.TabUnfocusedActive] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
            
        colors[(int) ImGuiCol.TitleBg] = new Vector4(0.15f, 0.155f, 0.151f, 1.0f);
        colors[(int) ImGuiCol.TitleBgActive] = new Vector4(0.15f, 0.155f, 0.151f, 1.0f);
        colors[(int) ImGuiCol.TitleBgCollapsed] = new Vector4(0.15f, 0.155f, 0.151f, 1.0f);
        
        
    }
}