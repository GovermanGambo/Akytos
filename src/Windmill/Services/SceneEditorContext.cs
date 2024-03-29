using System.IO;
using Akytos;
using Akytos.Diagnostics.Logging;
using Akytos.SceneSystems;
using Windmill.ProjectManagement;

namespace Windmill.Services;

internal class SceneEditorContext
{
    private readonly IProjectManager m_projectManager;
    private readonly SceneLoader m_sceneLoader;
    private readonly SpriteRendererSystem m_spriteRendererSystem;

    public SceneEditorContext(SceneTree sceneTree, SceneLoader sceneLoader, SpriteRendererSystem spriteRendererSystem, IProjectManager projectManager)
    {
        SceneTree = sceneTree;
        m_sceneLoader = sceneLoader;
        // TODO: This should maybe live inside a system registry for the scene tree.
        m_spriteRendererSystem = spriteRendererSystem;
        m_projectManager = projectManager;
    }

    public bool HasUnsavedChanges { get; set; }

    public string? CurrentSceneFilePath { get; private set; }
    public Node? SelectedNode { get; set; }

    public SceneTree SceneTree { get; }

    public void CreateNewScene<TNode>() where TNode : Node, new()
    {
        var rootNode = new TNode
        {
            Name = "RootNode"
        };

        SceneTree.SetScene(rootNode);
        SelectedNode = null;
        m_spriteRendererSystem.Context = rootNode;
        HasUnsavedChanges = true;

        CurrentSceneFilePath = null;
    }

    public bool TryLoadPreviousScene()
    {
        string? previousScene = m_projectManager.CurrentProject.EditorSettings.ReadString(SystemConstants.ConfigurationKeys.LastViewedScene);

        if (previousScene == null)
        {
            return false;
        }
        
        return LoadScene(previousScene);
    }

    public bool ReloadCurrentScene()
    {
        return CurrentSceneFilePath is not null && LoadScene(CurrentSceneFilePath);
    }
    
    public bool LoadScene(string filePath)
    {
        try
        {
            var scene = m_sceneLoader.LoadScene(filePath);
            SceneTree.SetScene(scene);
            SelectedNode = null;
            m_spriteRendererSystem.Context = SceneTree.CurrentScene;
            CurrentSceneFilePath = filePath;

            m_projectManager.CurrentProject.EditorSettings.WriteString(SystemConstants.ConfigurationKeys.LastViewedScene, filePath);
            m_projectManager.CurrentProject.EditorSettings.Save();

            Log.Core.Information("Loaded scene: {0}", filePath);

            return true;
        }
        catch (IOException e)
        {
             Log.Core.Error(e, "Failed to load scene {0}: {1}", filePath, e.Message);
            return false;
        }
    }

    public void SaveSceneAs(string filePath)
    {
        m_sceneLoader.SaveScene(filePath, SceneTree.CurrentScene);
        CurrentSceneFilePath = filePath;
        HasUnsavedChanges = false;

        m_projectManager.CurrentProject.EditorSettings.WriteString(SystemConstants.ConfigurationKeys.LastViewedScene, filePath);
        m_projectManager.CurrentProject.EditorSettings.Save();

        Log.Core.Information("Saved scene: {0}", filePath);
    }
}