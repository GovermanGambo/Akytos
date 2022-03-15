using Xunit;

namespace Akytos.Tests.Scenes;

public class NodePathTests
    {
        [Theory]
        [InlineData("/Node/Node2D:position:x")]
        [InlineData("./Node/Node2D")]
        [InlineData("/Node:position:x")]
        [InlineData(":position")]
        [InlineData("Node/Node2D:sprite")]
        public void NewNodePath_ShouldBeValid(string pathString)
        {
            var nodePath = new NodePath(pathString);
            Assert.False(nodePath.IsEmpty);
        }
        
        [Theory]
        [InlineData("/Node/Node2D:position:x/NodeYes")]
        [InlineData("../position:x")]
        [InlineData(":x:")]
        [InlineData("Node/Node2D:Sprite")]
        public void NewNodePath_ShouldBeInvalid(string pathString)
        {
            var nodePath = new NodePath(pathString);
            Assert.True(nodePath.IsEmpty);
        }

        [Theory]
        [InlineData("/Node/Node2D:position:x", true)]
        [InlineData("Node/Node2D", false)]
        [InlineData("./Node/Node2D", false)]
        public void NodePath_TestAbsolute(string pathString, bool isAbsolute)
        {
            var nodePath = new NodePath(pathString);
            Assert.Equal(isAbsolute, nodePath.IsAbsolute);
        }
        
        [Theory]
        [InlineData("/Node/Node2D:position:x", ":position:x")]
        [InlineData("Node2D:position", ":position")]
        [InlineData("./Node", "")]
        public void NodePath_TestPropertyPath(string pathString, string propertyPath)
        {
            var nodePath = new NodePath(pathString);
            Assert.Equal(propertyPath, nodePath.AsPropertyPath());
        }

        [Theory]
        [InlineData("/Node/Node2D:position:x", "Node/Node2D")]
        [InlineData("Node2D:position", "Node2D")]
        [InlineData("./Node", "Node")]
        [InlineData("./Node/Node2D:position:x", "Node/Node2D")]
        public void GetConcatenatedNames_Should_BeCorrect(string pathString, string concatenatedNames)
        {
            var nodePath = new NodePath(pathString);
            Assert.Equal(concatenatedNames, nodePath.GetConcatenatedNames());
        }
        
        [Theory]
        [InlineData("/Node/Node2D:position:x", "position:x")]
        [InlineData("Node2D:position", "position")]
        [InlineData("./Node", "")]
        public void GetConcatenatedSubNames_Should_BeCorrect(string pathString, string concatenatedNames)
        {
            var nodePath = new NodePath(pathString);
            Assert.Equal(concatenatedNames, nodePath.GetConcatenatedSubNames());
        }
        
        [Theory]
        [InlineData("/Node/Node2D:position:x", 0, "Node")]
        [InlineData("/Node/Node/Node2D:position", 2, "Node2D")]
        [InlineData("./Node", 0, "Node")]
        public void GetName_Should_BeCorrect(string pathString, int index, string name)
        {
            var nodePath = new NodePath(pathString);
            Assert.Equal(name, nodePath.GetName(index));
        }
    }