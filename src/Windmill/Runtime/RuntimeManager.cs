using System;
using Akytos.SceneSystems;
using Windmill.Modals;
using Windmill.Services;

namespace Windmill.Runtime;

internal class RuntimeManager
{
    private readonly SceneTree m_sceneTree;
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly EditorHotKeyService m_editorHotKeyService;
    private readonly ModalStack m_modalStack;

    public RuntimeManager(SceneTree sceneTree, EditorHotKeyService editorHotKeyService, SceneEditorContext sceneEditorContext, ModalStack modalStack)
    {
        m_sceneTree = sceneTree;
        m_editorHotKeyService = editorHotKeyService;
        m_sceneEditorContext = sceneEditorContext;
        m_modalStack = modalStack;
    }
    
    public bool IsGameRunning { get; private set; }

    public event Action? GameStarted;

    public void StartGame()
    {
        if (m_sceneEditorContext.CurrentSceneFilePath is null)
        {
            m_modalStack.PushModal<SaveSceneModal>();
        }
        else
        {
            m_sceneEditorContext.SaveSceneAs(m_sceneEditorContext.CurrentSceneFilePath);
        }

        if (m_sceneEditorContext.CurrentSceneFilePath is null)
        {
            return;
        }
        
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
        m_sceneEditorContext.ReloadCurrentScene();
    }
}