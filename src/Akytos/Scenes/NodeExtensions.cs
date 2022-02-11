using System.Numerics;

namespace Akytos
{
    public static class NodeExtensions
    {
        public static IEnumerable<TNode> GetChildren<TNode>(this Node node, bool includeRoot = false) where TNode : Node
        {
            var nodes = node.GetChildren(includeRoot, n => n is TNode);
            var spriteNodes = nodes.Select(n => (TNode)n);
            return spriteNodes;
        }

        public static bool IsAParentOf(this Node parent, Node node)
        {
            return parent.GetChildren().Any(n => n.GetPath() == node.GetPath());
        }

        public static Matrix4x4 GetTransform(this Node2D node)
        {
            return Matrix4x4.CreateScale(new Vector3(node.Scale, 1.0f))
                   * Matrix4x4.CreateRotationZ(node.Rotation)
                   * Matrix4x4.CreateTranslation(new Vector3(node.Position, 0.0f));
        }
    }
}