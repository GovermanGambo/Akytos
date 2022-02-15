using Akytos;

namespace Windmill.Services;

public class SceneEditorContext
{
    public SceneEditorContext(SceneTree sceneTree)
    {
        SceneTree = sceneTree;
    }
    
    public Node? SelectedNode { get; set; }

    public SceneTree SceneTree { get; }
}