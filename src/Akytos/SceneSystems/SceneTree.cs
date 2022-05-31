namespace Akytos.SceneSystems;

public sealed class SceneTree
{
    public SceneTree(ISystemsRegistry systems, SceneProcessMode processMode = SceneProcessMode.Runtime)
    {
        ProcessMode = processMode;
        Systems = systems;
    }

    public ISystemsRegistry Systems { get; }

    internal SceneProcessMode ProcessMode { get; }

    public Node CurrentScene { get; private set; } = null!;

    public int NodeCount => CurrentScene.GetChildren(true).Count();

    public event Action<Node>? NodeAdded;
    public event Action<Node>? NodeRemoved;
    public event Action? SceneStarting;
    public event Action? SceneEnding;

    public void OnNodeAdded(Node node)
    {
        if (ProcessMode == SceneProcessMode.Runtime) NodeAdded?.Invoke(node);
    }

    public void OnNodeRemoved(Node node)
    {
        if (ProcessMode == SceneProcessMode.Runtime) NodeRemoved?.Invoke(node);
    }

    public void SetScene(Node scene)
    {
        if (CurrentScene != null!)
        {
            CurrentScene.SceneTree = null;

            if (ProcessMode == SceneProcessMode.Runtime) Finish();
        }

        CurrentScene = scene;
        CurrentScene.SceneTree = this;

        if (ProcessMode == SceneProcessMode.Runtime) StartScene();
    }

    public void OnUpdate(DeltaTime deltaSeconds)
    {
        if (ProcessMode != SceneProcessMode.Runtime) return;

        Systems.OnUpdate(deltaSeconds);

        foreach (var node in CurrentScene.GetChildren(true)) node.OnUpdate(deltaSeconds);
    }

    public void OnRender()
    {
        Systems.OnRender();
    }

    private void Finish()
    {
        NodeRemoved?.Invoke(CurrentScene);
        SceneEnding?.Invoke();
    }

    private void StartScene()
    {
        NodeAdded?.Invoke(CurrentScene);
        SceneStarting?.Invoke();

        foreach (var child in CurrentScene.GetChildren(true)) child.OnReady();
    }
}