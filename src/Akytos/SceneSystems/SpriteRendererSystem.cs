using Akytos.Graphics;

namespace Akytos.SceneSystems;

public class SpriteRendererSystem : ISceneSystem
{
    private readonly ISpriteBatch m_spriteBatch;

    public SpriteRendererSystem(ISpriteBatch spriteBatch)
    {
        m_spriteBatch = spriteBatch;
    }

    public Node? Context { get; set; }

    public bool IsEnabled { get; set; } = true;

    public void OnUpdate(DeltaTime deltaTime)
    {
        
    }

    public void OnRender(ICamera camera)
    {
        if (Context == null)
        {
            return;
        }
        
        var nodes = Context.GetChildren<SpriteNode>(true);
        
        m_spriteBatch.Begin(camera);
        foreach (var spriteNode in nodes)
        {
            if (!spriteNode.IsEnabled || !spriteNode.IsVisible || spriteNode.Texture == null)
            {
                continue;
            }
            
            m_spriteBatch.Draw(spriteNode.Texture, spriteNode.GlobalPosition, spriteNode.GlobalScale, 
                spriteNode.GlobalRotation, spriteNode.Modulate, spriteNode.Id, spriteNode.IsCentered);
        }
        m_spriteBatch.End();
    }
}