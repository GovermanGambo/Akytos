using System.Numerics;
using Akytos;
using Akytos.Assets;
using Akytos.Editor;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Graphics.Buffers;
using Akytos.Layers;
using ImGuiNET;

namespace Windmill;

internal class EditorLayer : ILayer
{
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly IGraphicsResourceFactory m_graphicsResourceFactory;
    private readonly IEditorViewport m_editorViewport;
    private readonly SpriteBatch m_spriteBatch;
    
    private ITexture2D m_texture2D = null!;

    public EditorLayer(IGraphicsDevice graphicsDevice, IGraphicsResourceFactory graphicsResourceFactory, IEditorViewport editorViewport, SpriteBatch spriteBatch)
    {
        m_graphicsDevice = graphicsDevice;
        m_graphicsResourceFactory = graphicsResourceFactory;
        m_editorViewport = editorViewport;
        m_spriteBatch = spriteBatch;
    }

    public void Dispose()
    {
        
    }

    public bool IsEnabled { get; set; } = true;
    public void OnAttach()
    {
        m_texture2D = m_graphicsResourceFactory.CreateTexture2D(Asset.GetAssetPath("sprites/character_malePerson_idle.png"));
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        m_graphicsDevice.ClearColor(new Color(0.1f, 0.1f, 0.1f));
        m_graphicsDevice.Clear();

        m_spriteBatch.Begin(m_editorViewport.Camera);

        m_spriteBatch.Draw(m_texture2D, Vector2.Zero, Color.White);
        m_spriteBatch.Draw(m_texture2D, new Vector2(-68f, 0f), Color.White);
        
        m_spriteBatch.End();
    }

    public void OnEvent(IEvent e)
    {;
    }

    public void OnDrawGui()
    {
        Dockspace.Begin();
        
        ImGui.ShowDemoWindow();
        
        Dockspace.End();
    }
}