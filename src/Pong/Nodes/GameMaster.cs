using Akytos.SceneSystems;

namespace Pong.Nodes;

public class GameMaster : Node
{
    private List<PlayerController> m_players = new();

    public GameMaster()
    {
    }

    public GameMaster(string name)
        : base(name)
    {
    }

    public override void OnReady()
    {
        m_players = this.GetChildren<PlayerController>().ToList();
    }
}