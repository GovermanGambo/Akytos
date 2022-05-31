using System.Linq;
using Akytos.SceneSystems;
using Xunit;

namespace Akytos.Tests.Scenes;

public class NodeTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("Node")]
    public void CreateNode_Should_SetupProperly(string? name)
    {
        Node node;
        if (name == null)
        {
            node = new Node();
        }
        else
        {
            node = new Node(name);
        }

        name ??= "NewNode";
        
        Assert.Equal(name, node.Name);
        Assert.Null(node.SceneTree);
        Assert.Null(node.Owner);
        Assert.Null(node.GetPath());
    }

    [Fact]
    public void AddChildNodes_Should_SetupProperly()
    {
        var rootNode = new Node("RootNode");
        var childNodeA = new Node("ChildNodeA");
        var childNodeB = new Node("ChildNodeB");
        var childNodeC = new Node("ChildNodeC");

        rootNode.AddChild(childNodeA);
        rootNode.AddChild(childNodeB);
        childNodeA.AddChild(childNodeC);
        
        Assert.Null(rootNode.Owner);
        Assert.Null(childNodeA.Owner);
        Assert.Null(childNodeB.Owner);
        Assert.Null(childNodeC.Owner);

        Assert.Contains(childNodeA, rootNode.ImmediateChildren);
        Assert.Contains(childNodeB, rootNode.ImmediateChildren);
        Assert.Contains(childNodeC, childNodeA.ImmediateChildren);
    }

    [Fact]
    public void AddToSceneTree_Should_SetupProperly()
    {
        var rootNode = new Node("RootNode");
        var childNodeA = new Node("ChildNodeA");
        var childNodeB = new Node("ChildNodeB");
        var childNodeC = new Node("ChildNodeC");

        rootNode.AddChild(childNodeA);
        rootNode.AddChild(childNodeB);
        childNodeA.AddChild(childNodeC);

        rootNode.SceneTree = new SceneTree();
        
        Assert.Null(rootNode.Owner);
        Assert.Equal(rootNode, childNodeA.Owner);
        Assert.Equal(rootNode, childNodeB.Owner);
        Assert.Equal(childNodeA, childNodeC.Owner);
        
        Assert.True(rootNode.IsInsideTree);
        Assert.True(childNodeA.IsInsideTree);
        Assert.True(childNodeB.IsInsideTree);
        Assert.True(childNodeC.IsInsideTree);
        
        Assert.Equal(rootNode.SceneTree, childNodeA.SceneTree);
        Assert.Equal(rootNode.SceneTree, childNodeB.SceneTree);
        Assert.Equal(rootNode.SceneTree, childNodeC.SceneTree);
        
        Assert.Contains(childNodeA, rootNode.ImmediateChildren);
        Assert.Contains(childNodeB, rootNode.ImmediateChildren);
        Assert.Contains(childNodeC, childNodeA.ImmediateChildren);
        
        Assert.Equal("/RootNode", rootNode.GetPath());
        Assert.Equal("/RootNode/ChildNodeA", childNodeA.GetPath());
        Assert.Equal("/RootNode/ChildNodeB", childNodeB.GetPath());
        Assert.Equal("/RootNode/ChildNodeA/ChildNodeC", childNodeC.GetPath());
    }

    [Fact]
    public void RemoveFromSceneTree_Should_ResetProperly()
    {
        var rootNode = new Node("RootNode");
        var childNodeA = new Node("ChildNodeA");
        var childNodeB = new Node("ChildNodeB");
        var childNodeC = new Node("ChildNodeC");

        rootNode.AddChild(childNodeA);
        rootNode.AddChild(childNodeB);
        childNodeA.AddChild(childNodeC);
        
        rootNode.SceneTree = new SceneTree();
        rootNode.SceneTree = null;
        
        Assert.Null(rootNode.Owner);
        Assert.Null(childNodeA.Owner);
        Assert.Null(childNodeB.Owner);
        Assert.Null(childNodeC.Owner);

        Assert.Contains(childNodeA, rootNode.ImmediateChildren);
        Assert.Contains(childNodeB, rootNode.ImmediateChildren);
        Assert.Contains(childNodeC, childNodeA.ImmediateChildren);
    }

    [Fact]
    public void AddChild_Should_DisallowOwnedNodes()
    {
        var sceneTree = new SceneTree();
        
        var rootNode = new Node("RootNode");
        sceneTree.SetScene(rootNode);
        var newNode = new Node("Node");
        var result = newNode.AddChild(rootNode);
        
        Assert.Equal(Result.InvalidData, result);
    }
    
    [Fact]
    public void AddChild_Should_DisallowDuplicateNames()
    {
        var rootNode = new Node("RootNode");
        var childNodeA = new Node("Child");
        var childNodeB = new Node("Child");

        var resultA = rootNode.AddChild(childNodeA);
        var resultB = rootNode.AddChild(childNodeB);
        
        Assert.Equal(Result.Ok, resultA);
        Assert.Equal(Result.InvalidData, resultB);
    }
    
    [Fact]
    public void AddChild_Should_AllowIfSpecifiedDuplicateNames()
    {
        var rootNode = new Node("RootNode");
        var childNodeA = new Node("Child");
        var childNodeB = new Node("Child");

        var resultA = rootNode.AddChild(childNodeA);
        var resultB = rootNode.AddChild(childNodeB, true);
        
        Assert.Equal(Result.Ok, resultA);
        Assert.Equal(Result.Ok, resultB);
        Assert.Equal("Child_1", childNodeB.Name);
    }
    
    [Fact]
    public void GetPath_Should_BeEmptyIfNoSceneTree()
    {
        var newNode = new Node("Node");
        var nodePath = newNode.GetPath();
        
        Assert.Null(nodePath);
    }
    
    [Fact]
    public void GetNode_Should_ReturnCorrect()
    {
        var rootNode = new Node("TestNode");
        var sceneTree = new SceneTree();
        sceneTree.SetScene(rootNode);
        var childNode = new Node("ChildNode");
        var anotherChildNode = new Node("AnotherChild");
        var aThirdChildNode = new Node("AThirdChild");
        rootNode.AddChild(childNode);
        rootNode.AddChild(anotherChildNode);
        childNode.AddChild(aThirdChildNode);
        
        Assert.Equal(childNode, rootNode.GetNode(new NodePath("./ChildNode")));
        Assert.Equal(childNode, rootNode.GetNode(new NodePath("ChildNode")));
        Assert.Equal(anotherChildNode, rootNode.GetNode(new NodePath("./AnotherChild")));
        Assert.Equal(aThirdChildNode, rootNode.GetNode(new NodePath("./ChildNode/AThirdChild")));
        Assert.Equal(rootNode, rootNode.GetNode(new NodePath("")));
        Assert.Equal(aThirdChildNode, anotherChildNode.GetNode(new NodePath("/TestNode/ChildNode/AThirdChild")));
    }

    [Fact]
    public void RemoveChild_Should_RemoveNode()
    {
        var rootNode = new Node("RootNode");
        var childNode = new Node("ChildNode");
        rootNode.AddChild(childNode);

        var result = rootNode.RemoveChild(childNode);
        Assert.Equal(Result.Ok, result);
    }
    
    [Fact]
    public void RemoveChild_ShouldNot_RemoveMissingNode()
    {
        var rootNode = new Node("RootNode");
        var childNode = new Node("ChildNode");

        var result = rootNode.RemoveChild(childNode);
        Assert.Equal(Result.InvalidData, result);
    }
    
    [Fact]
    public void GetChildren_Should_WorkWithPredicate()
    {
        var rootNode = new Node("RootNode");
        var childNode = new Node("ChildNode");
        var anotherChildNode = new Node("AnotherChild");
        var aThirdChildNode = new Node("AThirdChild");
        rootNode.AddChild(childNode);
        rootNode.AddChild(anotherChildNode);
        childNode.AddChild(aThirdChildNode);
        
        var children = rootNode.GetChildren(false, n => n.Name == "ChildNode").ToList();

        Assert.Single(children);
        Assert.Equal(childNode, children.FirstOrDefault());
    }
    
    [Fact]
    public void GetChildren_Should_IncludeRootIfConfigured()
    {
        var rootNode = new Node("RootNode");
        var childNode = new Node("ChildNode");
        var anotherChildNode = new Node("AnotherChild");
        var aThirdChildNode = new Node("AThirdChild");
        rootNode.AddChild(childNode);
        rootNode.AddChild(anotherChildNode);
        childNode.AddChild(aThirdChildNode);

        var children = rootNode.GetChildren(true);
        
        Assert.Equal(rootNode, children.FirstOrDefault());
    }
}