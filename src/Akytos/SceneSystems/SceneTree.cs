﻿using Akytos.Graphics;

namespace Akytos.SceneSystems;

public sealed class SceneTree
{
    private bool m_isStarted;

    public SceneTree(ISystemsRegistry systems, SceneProcessMode processMode = SceneProcessMode.Runtime)
    {
        ProcessMode = processMode;
        Systems = systems;
    }

    public ISystemsRegistry Systems { get; }

    internal SceneProcessMode ProcessMode { get; set; }

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

            if (ProcessMode == SceneProcessMode.Runtime && m_isStarted) Finish();
        }

        CurrentScene = scene;
        CurrentScene.SceneTree = this;

        if (ProcessMode == SceneProcessMode.Runtime && m_isStarted) StartScene();
    }

    public void OnUpdate(DeltaTime deltaSeconds)
    {
        if (ProcessMode != SceneProcessMode.Runtime) return;

        Systems.OnUpdate(deltaSeconds);

        foreach (var node in CurrentScene.GetChildren(true)) node.OnUpdate(deltaSeconds);
    }

    public void OnRender(ICamera camera)
    {
        Systems.OnRender(camera);
    }

    public void Finish()
    {
        NodeRemoved?.Invoke(CurrentScene);
        SceneEnding?.Invoke();

        m_isStarted = false;
    }

    public void StartScene()
    {
        NodeAdded?.Invoke(CurrentScene);
        SceneStarting?.Invoke();

        foreach (var child in CurrentScene.GetChildren(true)) child.OnReady();

        m_isStarted = true;
    }
}