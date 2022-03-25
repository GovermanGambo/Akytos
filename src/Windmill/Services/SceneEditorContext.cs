using System;
using Akytos;
using Akytos.Assets;
using Akytos.Configuration;
using Akytos.ProjectManagement;

namespace Windmill.Services;

internal class SceneEditorContext
{
    private readonly SceneLoader m_sceneLoader;
    private readonly SpriteRendererSystem m_spriteRendererSystem;

    public SceneEditorContext(SceneTree sceneTree, SceneLoader sceneLoader, SpriteRendererSystem spriteRendererSystem)
    {
        SceneTree = sceneTree;
        m_sceneLoader = sceneLoader;
        // TODO: This should maybe live inside a system registry for the scene tree.
        m_spriteRendererSystem = spriteRendererSystem;
    }
    
    public bool HasUnsavedChanges { get; set; }
    
    public string? CurrentSceneFilename { get; private set; }
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

        CurrentSceneFilename = null;
    }

    public void LoadScene(string filePath)
    {
        var scene = m_sceneLoader.LoadScene(filePath);
        SceneTree.SetScene(scene);
        SelectedNode = null;
        m_spriteRendererSystem.Context = SceneTree.CurrentScene;
        CurrentSceneFilename = filePath;
        
        AkytosProject.CurrentProject?.Configuration.WriteString("InitialScene", filePath);
        AkytosProject.CurrentProject?.Configuration.Save();
        
        Debug.LogInformation("Loaded scene: {0}", filePath);
    }

    public void SaveSceneAs(string filePath)
    {
        m_sceneLoader.SaveScene(filePath, SceneTree.CurrentScene);
        CurrentSceneFilename = filePath;
        HasUnsavedChanges = false;
        
        AkytosProject.CurrentProject?.Configuration.WriteString("InitialScene", filePath);
        AkytosProject.CurrentProject?.Configuration.Save();
        
        Debug.LogInformation("Saved scene: {0}", filePath);
    }
}