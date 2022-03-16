using System;
using Akytos;
using Akytos.Assets;

namespace Windmill.Services;

internal class SceneEditorContext
{
    private readonly SceneLoader m_sceneLoader;
    private readonly SpriteRendererSystem m_spriteRendererSystem;
    private readonly IFileDialogService m_fileDialogService;

    public SceneEditorContext(SceneTree sceneTree, SceneLoader sceneLoader, SpriteRendererSystem spriteRendererSystem, IFileDialogService fileDialogService)
    {
        SceneTree = sceneTree;
        m_sceneLoader = sceneLoader;
        // TODO: This should maybe live inside a system registry for the scene tree.
        m_spriteRendererSystem = spriteRendererSystem;
        m_fileDialogService = fileDialogService;
    }
    
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
    }

    public void LoadScene(string filePath)
    {
        var scene = m_sceneLoader.LoadScene(filePath);
        SceneTree.SetScene(scene);
        CurrentSceneFilename = filePath;
    }

    public void SaveSceneAs(string filePath)
    {
        m_sceneLoader.SaveScene(filePath, SceneTree.CurrentScene);
        CurrentSceneFilename = filePath;
    }
}