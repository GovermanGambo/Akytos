using System;
using Akytos;
using Akytos.Assets;

namespace Windmill.Services;

internal class SceneEditorContext
{
    private readonly SceneLoader m_sceneLoader;
    private readonly SpriteRendererSystem m_spriteRendererSystem;
    private readonly AppConfiguration m_appConfiguration;

    public SceneEditorContext(SceneTree sceneTree, SceneLoader sceneLoader, SpriteRendererSystem spriteRendererSystem, AppConfiguration appConfiguration)
    {
        SceneTree = sceneTree;
        m_sceneLoader = sceneLoader;
        // TODO: This should maybe live inside a system registry for the scene tree.
        m_spriteRendererSystem = spriteRendererSystem;
        m_appConfiguration = appConfiguration;
    }
    
    public bool HasUnsavedChanges { get; set; }
    
    public string? CurrentSceneFilename { get; private set; }
    public Node? SelectedNode { get; set; }

    public SceneTree SceneTree { get; }

    public bool RemoveNode(Node node)
    {
        if (node.Owner == null)
        {
            return false;
        }
        
        var result = node.Owner.RemoveChild(node, true);
        if (SelectedNode == node)
        {
            SelectedNode = null;
        }

        return result == Result.Ok;
    }

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
        
        m_appConfiguration.WriteString("initialScene", filePath);
        m_appConfiguration.Save();
        
        Debug.LogInformation("Loaded scene: {0}", filePath);
    }

    public void SaveSceneAs(string filePath)
    {
        m_sceneLoader.SaveScene(filePath, SceneTree.CurrentScene);
        CurrentSceneFilename = filePath;
        HasUnsavedChanges = false;
        
        m_appConfiguration.WriteString("initialScene", filePath);
        m_appConfiguration.Save();
        
        Debug.LogInformation("Saved scene: {0}", filePath);
    }
}