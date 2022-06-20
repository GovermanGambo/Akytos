using System;
using Akytos.SceneSystems;
using Windmill.Services;

namespace Windmill.Runtime;

internal class RuntimeManager
{
    private readonly SceneTree m_sceneTree;
    private readonly EditorHotKeyService m_editorHotKeyService;

    public RuntimeManager(SceneTree sceneTree, EditorHotKeyService editorHotKeyService)
    {
        m_sceneTree = sceneTree;
        m_editorHotKeyService = editorHotKeyService;
    }
    
    public bool IsGameRunning { get; private set; }

    public event Action? GameStarted;

    public void StartGame()
    {
        m_sceneTree.ProcessMode = SceneProcessMode.Runtime;
        m_editorHotKeyService.IsEnabled = false;
        IsGameRunning = true;
        m_sceneTree.StartScene();
        GameStarted?.Invoke();
    }

    public void StopGame()
    {
        m_editorHotKeyService.IsEnabled = true;
        IsGameRunning = false;
        m_sceneTree.ProcessMode = SceneProcessMode.Editor;
        m_sceneTree.Finish();
    }
}