using System.Collections.Generic;
using Xunit;

namespace Akytos.Tests.Scenes;

public class NodeTests
{
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

    private void AssertSerializedNode(SerializedObject serializedObject)
    {
        Assert.Equal(2, serializedObject.Fields.Length);
        // Children
        Assert.IsType<List<object>>(serializedObject.Fields[0].Value);
        // Name
        Assert.IsType<string>(serializedObject.Fields[1].Value);
    }
}