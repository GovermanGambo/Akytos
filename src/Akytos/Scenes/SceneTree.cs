namespace Akytos;

public sealed class SceneTree
{
    public SceneTree()
    {
    }

    public Node CurrentScene { get; private set; } = null!;

    public int NodeCount => CurrentScene.GetChildren(true).Count();

    public event Action<Node>? NodeAdded;
    public event Action<Node>? NodeRemoved;

    public void OnNodeAdded(Node node)
    {
        NodeAdded?.Invoke(node);
    }

    public void OnNodeRemoved(Node node)
    {
        NodeRemoved?.Invoke(node);
    }

    public void SetScene(Node scene)
    {
        if (CurrentScene != null!) NodeRemoved?.Invoke(CurrentScene);

        CurrentScene = scene;
        CurrentScene.SceneTree = this;

        NodeAdded?.Invoke(CurrentScene);
    }

    public void OnUpdate(float deltaSeconds)
    {
        foreach (var node in CurrentScene.GetChildren(true)) node.OnUpdate(deltaSeconds);
    }

    public void OnRender(float deltaSeconds)
    {
    }

    private void Initialize()
    {
        foreach (var node in CurrentScene.GetChildren(true)) node.OnReady();
    }

    private void Finish()
    {
    }
}