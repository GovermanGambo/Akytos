using Akytos.Graphics;

namespace Akytos;

internal class SpriteRendererSystem
{
    private readonly SpriteBatch m_spriteBatch;

    public SpriteRendererSystem(SpriteBatch spriteBatch)
    {
        m_spriteBatch = spriteBatch;
    }

    public Node Context { get; set; }
    public ICamera Camera { get; set; }

    public void OnUpdate(DeltaTime deltaTime)
    {
        var nodes = Context.GetChildren<SpriteNode>(true);
        
        m_spriteBatch.Begin(Camera);
        foreach (var spriteNode in nodes)
        {
            if (spriteNode.Texture == null)
            {
                continue;
            }
            
            m_spriteBatch.Draw(spriteNode.Texture, spriteNode.GlobalPosition);
        }
        m_spriteBatch.End();
    }
}