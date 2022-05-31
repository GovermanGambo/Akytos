namespace Akytos.SceneSystems;

public class Physics2DSystem : ISceneSystem
{
    private readonly SceneTree m_sceneTree;

    public Physics2DSystem(SceneTree sceneTree)
    {
        m_sceneTree = sceneTree;
    }

    public bool IsEnabled { get; set; } = true;
    public void OnUpdate(DeltaTime time)
    {
        // TODO: Properly implement AABB collision
        var physicsNodes = m_sceneTree.CurrentScene.GetChildren<PhysicsObject2D>().ToList();

        foreach (var physicsNode in physicsNodes)
        {
            bool willCollide = false;
            if (physicsNode.Shape is not null)
            {
                foreach (var otherNode in physicsNodes)
                {
                    if (otherNode == physicsNode || otherNode.Shape is null)
                    {
                        continue;
                    }
                    
                    if (physicsNode.Shape.CollideWithMotion(physicsNode.GetTransform(), physicsNode.Velocity, otherNode.Shape,
                            otherNode.GetTransform()))
                    {
                        willCollide = true;
                    }
                }
            }

            if (!willCollide)
            {
                physicsNode.Position += physicsNode.Velocity;
            }
        }
    }

    public void OnRender()
    {
    }
}