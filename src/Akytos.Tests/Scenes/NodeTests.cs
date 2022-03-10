using System.Collections.Generic;
using System.Numerics;
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
    public void SerializeNode_Should_BeCreatedProperly()
    {
        var node = new Node("MyNewNode");
        var childNode = new Node("ChildNode");
        node.AddChild(childNode);

        var serializedObject = SerializedObject.Create(node);
        
        Assert.NotNull(serializedObject);
        AssertSerializedNode(serializedObject);

        var children = serializedObject.Fields[0].Value as List<object>;
        foreach (object child in children)
        {
            AssertSerializedNode((SerializedObject)child);
        }
    }

    [Fact]
    public void DeserializeNode_Should_DeserializeProperly()
    {
        var rootNode = new Node("RootNode");
        var childNodeA = new Node("ChildNodeA");
        var childNodeB = new Node2D("ChildNodeB")
        {
            Position = Vector2.One,
            Scale = new Vector2(0.5f, 0.75f)
        };
        var childNodeC = new Node2D("ChildNodeC");

        rootNode.AddChild(childNodeA);
        rootNode.AddChild(childNodeB);
        childNodeA.AddChild(childNodeC);

        var serializedObject = SerializedObject.Create(rootNode);

        var deserializedObject = SerializedObject.Deserialize(serializedObject) as Node;
        
        Assert.NotNull(deserializedObject);
        
        Assert.Equal("RootNode", deserializedObject.Name);
        Assert.Equal("ChildNodeA", childNodeA.Name);
        Assert.Equal("ChildNodeB", childNodeB.Name);
        Assert.Equal("ChildNodeC", childNodeC.Name);
        
        Assert.Null(rootNode.Owner);
        Assert.Null(childNodeA.Owner);
        Assert.Null(childNodeB.Owner);
        Assert.Null(childNodeC.Owner);

        Assert.Contains(childNodeA, rootNode.ImmediateChildren);
        Assert.Contains(childNodeB, rootNode.ImmediateChildren);
        Assert.Contains(childNodeC, childNodeA.ImmediateChildren);
        
        Assert.Equal(Vector2.One, childNodeB.Position);
        Assert.Equal(new Vector2(0.5f, 0.75f), childNodeB.Scale);
    }

    [Fact]
    public void YamlDeserializeNode_Should_WorkProperly()
    {
        var rootNode = new Node("RootNode");
        var childNodeA = new Node("ChildNodeA");
        var childNodeB = new Node2D("ChildNodeB")
        {
            Position = Vector2.One,
            Scale = new Vector2(0.5f, 0.75f)
        };
        var childNodeC = new Node2D("ChildNodeC");

        rootNode.AddChild(childNodeA);
        rootNode.AddChild(childNodeB);
        childNodeA.AddChild(childNodeC);

        var serializer = new YamlSerializer();
        var deserializer = new YamlDeserializer();

        string yaml = serializer.Serialize(rootNode);
        var deserializedNode = deserializer.Deserialize<Node>(yaml);
        
        Assert.NotNull(deserializedNode);
    }

    private static void AssertSerializedNode(SerializedObject serializedObject)
    {
        Assert.Equal(2, serializedObject.Fields.Length);
        // Children
        Assert.IsType<List<object>>(serializedObject.Fields[0].Value);
        // Name
        Assert.IsType<string>(serializedObject.Fields[1].Value);
    }
}