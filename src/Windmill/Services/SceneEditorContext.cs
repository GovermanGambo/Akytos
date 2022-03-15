using Akytos;
using Akytos.Assets;

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
        m_spriteRendererSystem.Context = rootNode;
    }

    public void LoadScene()
    {
        
    }

    public void SaveSceneAs()
    {
        
    }

    public void SaveScene()
    {
        try
        {
            if (CurrentSceneFilename == null)
            {
                SaveSceneAs();
            }
            else
            {
                string filePath = Asset.GetAssetPath(CurrentSceneFilename);
                m_sceneLoader.SaveScene(filePath, SceneTree.CurrentScene);
            }
        }
        catch (Exception e)
        {
            // TODO: Show error dialog
            Console.WriteLine(e);
            throw;
        }
    }
}