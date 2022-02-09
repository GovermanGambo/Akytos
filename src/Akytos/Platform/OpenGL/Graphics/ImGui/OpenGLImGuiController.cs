using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Akytos.Graphics;

public class OpenGLImGuiController : IImGuiController
{
    private readonly List<char> m_pressedChars = new();
    private int m_attribLocationProjMtx;

    private int m_attribLocationTex;
    private int m_attribLocationVtxColor;
    private int m_attribLocationVtxPos;
    private int m_attribLocationVtxUv;
    private uint m_elementsHandle;

    private ImGuiTexture m_fontImGuiTexture;
    private bool m_frameBegun;
    private GL m_gl;
    private Version m_glVersion;
    private ImGuiShader m_imGuiShader;
    private IInputContext m_input;
    private IKeyboard m_keyboard;
    private uint m_vboHandle;
    private uint m_vertexArrayObject;
    private IView m_view;
    private int m_windowHeight;

    private int m_windowWidth;

    /// <summary>
    ///     Constructs a new ImGuiController.
    /// </summary>
    public OpenGLImGuiController(GL gl, IView view, IInputContext input) : this(gl, view, input, null, null)
    {
    }

    /// <summary>
    ///     Constructs a new ImGuiController with font configuration.
    /// </summary>
    public OpenGLImGuiController(GL gl, IView view, IInputContext input, ImGuiFontConfig imGuiFontConfig) : this(gl,
        view,
        input, imGuiFontConfig, null)
    {
    }

    /// <summary>
    ///     Constructs a new ImGuiController with an onConfigureIO Action.
    /// </summary>
    public OpenGLImGuiController(GL gl, IView view, IInputContext input, Action onConfigureIO) : this(gl, view, input,
        null, onConfigureIO)
    {
    }

    /// <summary>
    ///     Constructs a new ImGuiController with font configuration and onConfigure Action.
    /// </summary>
    public OpenGLImGuiController(GL gl, IView view, IInputContext input, ImGuiFontConfig? imGuiFontConfig = null,
        Action onConfigureIO = null)
    {
        Init(gl, view, input);

        var io = ImGuiNET.ImGui.GetIO();
        if (imGuiFontConfig is not null)
            io.Fonts.AddFontFromFileTTF(imGuiFontConfig.Value.FontPath, imGuiFontConfig.Value.FontSize);

        onConfigureIO?.Invoke();

        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        CreateDeviceResources();
        SetKeyMappings();

        SetPerFrameImGuiData(1f / 60f);

        BeginFrame();
    }

    /// <summary>
    ///     Frees all graphics resources used by the renderer.
    /// </summary>
    public void Dispose()
    {
        m_view.Resize -= WindowResized;
        m_keyboard.KeyChar -= OnKeyChar;

        m_gl.DeleteBuffer(m_vboHandle);
        m_gl.DeleteBuffer(m_elementsHandle);
        m_gl.DeleteVertexArray(m_vertexArrayObject);

        m_fontImGuiTexture.Dispose();
        m_imGuiShader.Dispose();
    }

    private void Init(GL gl, IView view, IInputContext input)
    {
        m_gl = gl;
        m_glVersion = new Version(gl.GetInteger(GLEnum.MajorVersion), gl.GetInteger(GLEnum.MinorVersion));
        m_view = view;
        m_input = input;
        m_windowWidth = view.Size.X;
        m_windowHeight = view.Size.Y;

        var context = ImGuiNET.ImGui.CreateContext();
        ImGuiNET.ImGui.SetCurrentContext(context);
        ImGuiNET.ImGui.StyleColorsDark();
    }

    private void BeginFrame()
    {
        ImGuiNET.ImGui.NewFrame();
        m_frameBegun = true;
        m_keyboard = m_input.Keyboards[0];
        m_view.Resize += WindowResized;
        m_keyboard.KeyChar += OnKeyChar;
    }

    private void OnKeyChar(IKeyboard arg1, char arg2)
    {
        m_pressedChars.Add(arg2);
    }

    private void WindowResized(Vector2D<int> size)
    {
        m_windowWidth = size.X;
        m_windowHeight = size.Y;
    }

    /// <summary>
    ///     Renders the ImGui draw list data.
    ///     This method requires a <see cref="GraphicsDevice" /> because it may create new DeviceBuffers if the size of vertex
    ///     or index data has increased beyond the capacity of the existing buffers.
    ///     A <see cref="CommandList" /> is needed to submit drawing and resource update commands.
    /// </summary>
    public void Render()
    {
        if (m_frameBegun)
        {
            m_frameBegun = false;
            ImGuiNET.ImGui.Render();
            RenderImDrawData(ImGuiNET.ImGui.GetDrawData());
        }
    }

    /// <summary>
    ///     Updates ImGui input and IO configuration state.
    /// </summary>
    public void Update(float deltaSeconds)
    {
        if (m_frameBegun) ImGuiNET.ImGui.Render();

        SetPerFrameImGuiData(deltaSeconds);
        UpdateImGuiInput();

        m_frameBegun = true;
        ImGuiNET.ImGui.NewFrame();
    }

    /// <summary>
    ///     Sets per-frame data based on the associated window.
    ///     This is called by Update(float).
    /// </summary>
    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        var io = ImGuiNET.ImGui.GetIO();
        io.DisplaySize = new Vector2(m_windowWidth, m_windowHeight);

        if (m_windowWidth > 0 && m_windowHeight > 0)
            io.DisplayFramebufferScale = new Vector2(m_view.FramebufferSize.X / m_windowWidth,
                m_view.FramebufferSize.Y / m_windowHeight);

        io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
    }

    private void UpdateImGuiInput()
    {
        var io = ImGuiNET.ImGui.GetIO();

        var mouseState = m_input.Mice[0];
        var keyboardState = m_input.Keyboards[0];

        io.MouseDown[0] = mouseState.IsButtonPressed(Silk.NET.Input.MouseButton.Left);
        io.MouseDown[1] = mouseState.IsButtonPressed(Silk.NET.Input.MouseButton.Right);
        io.MouseDown[2] = mouseState.IsButtonPressed(Silk.NET.Input.MouseButton.Middle);

        var point = new Point((int)mouseState.Position.X, (int)mouseState.Position.Y);
        io.MousePos = new Vector2(point.X, point.Y);

        var wheel = mouseState.ScrollWheels[0];
        io.MouseWheel = wheel.Y;
        io.MouseWheelH = wheel.X;

        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            if (key == Key.Unknown) continue;
            io.KeysDown[(int)key] = keyboardState.IsKeyPressed(key);
        }

        foreach (var c in m_pressedChars) io.AddInputCharacter(c);

        m_pressedChars.Clear();

        io.KeyCtrl = keyboardState.IsKeyPressed(Key.ControlLeft) || keyboardState.IsKeyPressed(Key.ControlRight);
        io.KeyAlt = keyboardState.IsKeyPressed(Key.AltLeft) || keyboardState.IsKeyPressed(Key.AltRight);
        io.KeyShift = keyboardState.IsKeyPressed(Key.ShiftLeft) || keyboardState.IsKeyPressed(Key.ShiftRight);
        io.KeySuper = keyboardState.IsKeyPressed(Key.SuperLeft) || keyboardState.IsKeyPressed(Key.SuperRight);
    }

    internal void PressChar(char keyChar)
    {
        m_pressedChars.Add(keyChar);
    }

    private static void SetKeyMappings()
    {
        var io = ImGuiNET.ImGui.GetIO();
        io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown;
        io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home;
        io.KeyMap[(int)ImGuiKey.End] = (int)Key.End;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.Backspace;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape;
        io.KeyMap[(int)ImGuiKey.A] = (int)Key.A;
        io.KeyMap[(int)ImGuiKey.C] = (int)Key.C;
        io.KeyMap[(int)ImGuiKey.V] = (int)Key.V;
        io.KeyMap[(int)ImGuiKey.X] = (int)Key.X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z;
    }

    private unsafe void SetupRenderState(ImDrawDataPtr drawDataPtr, int framebufferWidth, int framebufferHeight)
    {
        // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, polygon fill
        m_gl.Enable(GLEnum.Blend);
        m_gl.BlendEquation(GLEnum.FuncAdd);
        m_gl.BlendFuncSeparate(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha, GLEnum.One, GLEnum.OneMinusSrcAlpha);
        m_gl.Disable(GLEnum.CullFace);
        m_gl.Disable(GLEnum.DepthTest);
        m_gl.Disable(GLEnum.StencilTest);
        m_gl.Enable(GLEnum.ScissorTest);
#if !GLES
        m_gl.Disable(GLEnum.PrimitiveRestart);
        m_gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Fill);
#endif

        var L = drawDataPtr.DisplayPos.X;
        var R = drawDataPtr.DisplayPos.X + drawDataPtr.DisplaySize.X;
        var T = drawDataPtr.DisplayPos.Y;
        var B = drawDataPtr.DisplayPos.Y + drawDataPtr.DisplaySize.Y;

        Span<float> orthoProjection = stackalloc float[]
        {
            2.0f / (R - L), 0.0f, 0.0f, 0.0f,
            0.0f, 2.0f / (T - B), 0.0f, 0.0f,
            0.0f, 0.0f, -1.0f, 0.0f,
            (R + L) / (L - R), (T + B) / (B - T), 0.0f, 1.0f
        };

        m_imGuiShader.UseShader();
        m_gl.Uniform1(m_attribLocationTex, 0);
        m_gl.UniformMatrix4(m_attribLocationProjMtx, 1, false, orthoProjection);
        m_gl.CheckGlError("Projection");

        m_gl.BindSampler(0, 0);

        // Setup desired GL state
        // Recreate the VAO every time (this is to easily allow multiple GL contexts to be rendered to. VAO are not shared among GL contexts)
        // The renderer would actually work without any VAO bound, but then our VertexAttrib calls would overwrite the default one currently bound.
        m_vertexArrayObject = m_gl.GenVertexArray();
        m_gl.BindVertexArray(m_vertexArrayObject);
        m_gl.CheckGlError("VAO");

        // Bind vertex/index buffers and setup attributes for ImDrawVert
        m_gl.BindBuffer(GLEnum.ArrayBuffer, m_vboHandle);
        m_gl.BindBuffer(GLEnum.ElementArrayBuffer, m_elementsHandle);
        m_gl.EnableVertexAttribArray((uint)m_attribLocationVtxPos);
        m_gl.EnableVertexAttribArray((uint)m_attribLocationVtxUv);
        m_gl.EnableVertexAttribArray((uint)m_attribLocationVtxColor);
        m_gl.VertexAttribPointer((uint)m_attribLocationVtxPos, 2, GLEnum.Float, false, (uint)sizeof(ImDrawVert),
            (void*)0);
        m_gl.VertexAttribPointer((uint)m_attribLocationVtxUv, 2, GLEnum.Float, false, (uint)sizeof(ImDrawVert),
            (void*)8);
        m_gl.VertexAttribPointer((uint)m_attribLocationVtxColor, 4, GLEnum.UnsignedByte, true,
            (uint)sizeof(ImDrawVert), (void*)16);
    }

    private unsafe void RenderImDrawData(ImDrawDataPtr drawDataPtr)
    {
        var framebufferWidth = (int)(drawDataPtr.DisplaySize.X * drawDataPtr.FramebufferScale.X);
        var framebufferHeight = (int)(drawDataPtr.DisplaySize.Y * drawDataPtr.FramebufferScale.Y);
        if (framebufferWidth <= 0 || framebufferHeight <= 0)
            return;

        // Backup GL state
        m_gl.GetInteger(GLEnum.ActiveTexture, out var lastActiveTexture);
        m_gl.ActiveTexture(GLEnum.Texture0);

        m_gl.GetInteger(GLEnum.CurrentProgram, out var lastProgram);
        m_gl.GetInteger(GLEnum.TextureBinding2D, out var lastTexture);

        m_gl.GetInteger(GLEnum.SamplerBinding, out var lastSampler);

        m_gl.GetInteger(GLEnum.ArrayBufferBinding, out var lastArrayBuffer);
        m_gl.GetInteger(GLEnum.VertexArrayBinding, out var lastVertexArrayObject);

#if !GLES
        Span<int> lastPolygonMode = stackalloc int[2];
        m_gl.GetInteger(GLEnum.PolygonMode, lastPolygonMode);
#endif

        Span<int> lastScissorBox = stackalloc int[4];
        m_gl.GetInteger(GLEnum.ScissorBox, lastScissorBox);

        m_gl.GetInteger(GLEnum.BlendSrcRgb, out var lastBlendSrcRgb);
        m_gl.GetInteger(GLEnum.BlendDstRgb, out var lastBlendDstRgb);

        m_gl.GetInteger(GLEnum.BlendSrcAlpha, out var lastBlendSrcAlpha);
        m_gl.GetInteger(GLEnum.BlendDstAlpha, out var lastBlendDstAlpha);

        m_gl.GetInteger(GLEnum.BlendEquationRgb, out var lastBlendEquationRgb);
        m_gl.GetInteger(GLEnum.BlendEquationAlpha, out var lastBlendEquationAlpha);

        var lastEnableBlend = m_gl.IsEnabled(GLEnum.Blend);
        var lastEnableCullFace = m_gl.IsEnabled(GLEnum.CullFace);
        var lastEnableDepthTest = m_gl.IsEnabled(GLEnum.DepthTest);
        var lastEnableStencilTest = m_gl.IsEnabled(GLEnum.StencilTest);
        var lastEnableScissorTest = m_gl.IsEnabled(GLEnum.ScissorTest);

#if !GLES
        var lastEnablePrimitiveRestart = m_gl.IsEnabled(GLEnum.PrimitiveRestart);
#endif

        SetupRenderState(drawDataPtr, framebufferWidth, framebufferHeight);

        // Will project scissor/clipping rectangles into framebuffer space
        var clipOff = drawDataPtr.DisplayPos; // (0,0) unless using multi-viewports
        var clipScale = drawDataPtr.FramebufferScale; // (1,1) unless using retina display which are often (2,2)

        // Render command lists
        for (var n = 0; n < drawDataPtr.CmdListsCount; n++)
        {
            var cmdListPtr = drawDataPtr.CmdListsRange[n];

            // Upload vertex/index buffers

            m_gl.BufferData(GLEnum.ArrayBuffer, (nuint)(cmdListPtr.VtxBuffer.Size * sizeof(ImDrawVert)),
                (void*)cmdListPtr.VtxBuffer.Data, GLEnum.StreamDraw);
            m_gl.CheckGlError($"Data Vert {n}");
            m_gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(cmdListPtr.IdxBuffer.Size * sizeof(ushort)),
                (void*)cmdListPtr.IdxBuffer.Data, GLEnum.StreamDraw);
            m_gl.CheckGlError($"Data Idx {n}");

            for (var cmd_i = 0; cmd_i < cmdListPtr.CmdBuffer.Size; cmd_i++)
            {
                var cmdPtr = cmdListPtr.CmdBuffer[cmd_i];

                if (cmdPtr.UserCallback != IntPtr.Zero) throw new NotImplementedException();

                Vector4 clipRect;
                clipRect.X = (cmdPtr.ClipRect.X - clipOff.X) * clipScale.X;
                clipRect.Y = (cmdPtr.ClipRect.Y - clipOff.Y) * clipScale.Y;
                clipRect.Z = (cmdPtr.ClipRect.Z - clipOff.X) * clipScale.X;
                clipRect.W = (cmdPtr.ClipRect.W - clipOff.Y) * clipScale.Y;

                if (clipRect.X < framebufferWidth && clipRect.Y < framebufferHeight && clipRect.Z >= 0.0f &&
                    clipRect.W >= 0.0f)
                {
                    // Apply scissor/clipping rectangle
                    m_gl.Scissor((int)clipRect.X, (int)(framebufferHeight - clipRect.W),
                        (uint)(clipRect.Z - clipRect.X), (uint)(clipRect.W - clipRect.Y));
                    m_gl.CheckGlError("Scissor");

                    // Bind texture, Draw
                    m_gl.BindTexture(GLEnum.Texture2D, (uint)cmdPtr.TextureId);
                    m_gl.CheckGlError("Texture");

                    m_gl.DrawElementsBaseVertex(GLEnum.Triangles, cmdPtr.ElemCount, GLEnum.UnsignedShort,
                        (void*)(cmdPtr.IdxOffset * sizeof(ushort)), (int)cmdPtr.VtxOffset);
                    m_gl.CheckGlError("Draw");
                }
            }
        }

        // Destroy the temporary VAO
        m_gl.DeleteVertexArray(m_vertexArrayObject);
        m_vertexArrayObject = 0;

        // Restore modified GL state
        m_gl.UseProgram((uint)lastProgram);
        m_gl.BindTexture(GLEnum.Texture2D, (uint)lastTexture);

        m_gl.BindSampler(0, (uint)lastSampler);

        m_gl.ActiveTexture((GLEnum)lastActiveTexture);

        m_gl.BindVertexArray((uint)lastVertexArrayObject);

        m_gl.BindBuffer(GLEnum.ArrayBuffer, (uint)lastArrayBuffer);
        m_gl.BlendEquationSeparate((GLEnum)lastBlendEquationRgb, (GLEnum)lastBlendEquationAlpha);
        m_gl.BlendFuncSeparate((GLEnum)lastBlendSrcRgb, (GLEnum)lastBlendDstRgb, (GLEnum)lastBlendSrcAlpha,
            (GLEnum)lastBlendDstAlpha);

        if (lastEnableBlend)
            m_gl.Enable(GLEnum.Blend);
        else
            m_gl.Disable(GLEnum.Blend);

        if (lastEnableCullFace)
            m_gl.Enable(GLEnum.CullFace);
        else
            m_gl.Disable(GLEnum.CullFace);

        if (lastEnableDepthTest)
            m_gl.Enable(GLEnum.DepthTest);
        else
            m_gl.Disable(GLEnum.DepthTest);
        if (lastEnableStencilTest)
            m_gl.Enable(GLEnum.StencilTest);
        else
            m_gl.Disable(GLEnum.StencilTest);

        if (lastEnableScissorTest)
            m_gl.Enable(GLEnum.ScissorTest);
        else
            m_gl.Disable(GLEnum.ScissorTest);

#if !GLES
        if (lastEnablePrimitiveRestart)
            m_gl.Enable(GLEnum.PrimitiveRestart);
        else
            m_gl.Disable(GLEnum.PrimitiveRestart);

        m_gl.PolygonMode(GLEnum.FrontAndBack, (GLEnum)lastPolygonMode[0]);
#endif

        m_gl.Scissor(lastScissorBox[0], lastScissorBox[1], (uint)lastScissorBox[2], (uint)lastScissorBox[3]);
    }

    private void CreateDeviceResources()
    {
        // Backup GL state

        m_gl.GetInteger(GLEnum.TextureBinding2D, out var lastTexture);
        m_gl.GetInteger(GLEnum.ArrayBufferBinding, out var lastArrayBuffer);
        m_gl.GetInteger(GLEnum.VertexArrayBinding, out var lastVertexArray);

        var vertexSource =
            @"#version 330
        layout (location = 0) in vec2 Position;
        layout (location = 1) in vec2 UV;
        layout (location = 2) in vec4 Color;
        uniform mat4 ProjMtx;
        out vec2 Frag_UV;
        out vec4 Frag_Color;
        void main()
        {
            Frag_UV = UV;
            Frag_Color = Color;
            gl_Position = ProjMtx * vec4(Position.xy,0,1);
        }";
        var fragmentSource =
            @"#version 330
        in vec2 Frag_UV;
        in vec4 Frag_Color;
        uniform sampler2D Texture;
        layout (location = 0) out vec4 Out_Color;
        void main()
        {
            Out_Color = Frag_Color * texture(Texture, Frag_UV.st);
        }";

        m_imGuiShader = new ImGuiShader(m_gl, vertexSource, fragmentSource);

        m_attribLocationTex = m_imGuiShader.GetUniformLocation("Texture");
        m_attribLocationProjMtx = m_imGuiShader.GetUniformLocation("ProjMtx");
        m_attribLocationVtxPos = m_imGuiShader.GetAttribLocation("Position");
        m_attribLocationVtxUv = m_imGuiShader.GetAttribLocation("UV");
        m_attribLocationVtxColor = m_imGuiShader.GetAttribLocation("Color");

        m_vboHandle = m_gl.GenBuffer();
        m_elementsHandle = m_gl.GenBuffer();

        RecreateFontDeviceTexture();

        // Restore modified GL state
        m_gl.BindTexture(GLEnum.Texture2D, (uint)lastTexture);
        m_gl.BindBuffer(GLEnum.ArrayBuffer, (uint)lastArrayBuffer);

        m_gl.BindVertexArray((uint)lastVertexArray);

        m_gl.CheckGlError("End of ImGui setup");
    }

    /// <summary>
    ///     Creates the texture used to render text.
    /// </summary>
    private void RecreateFontDeviceTexture()
    {
        // Build texture atlas
        var io = ImGuiNET.ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out var width, out var height,
            out var bytesPerPixel); // Load as RGBA 32-bit (75% of the memory is wasted, but default font is so small) because it is more likely to be compatible with user's existing shaders. If your ImTextureId represent a higher-level concept than just a GL texture id, consider calling GetTexDataAsAlpha8() instead to save on GPU memory.

        // Upload texture to graphics system
        m_gl.GetInteger(GLEnum.Texture2D, out var lastTexture);

        m_fontImGuiTexture = new ImGuiTexture(m_gl, width, height, pixels);
        m_fontImGuiTexture.Bind();
        m_fontImGuiTexture.SetMagFilter(TextureMagFilter.Linear);
        m_fontImGuiTexture.SetMinFilter(TextureMinFilter.Linear);

        // Store our identifier
        io.Fonts.SetTexID((IntPtr)m_fontImGuiTexture.GlTexture);

        // Restore state
        m_gl.BindTexture(GLEnum.Texture2D, (uint)lastTexture);
    }
}