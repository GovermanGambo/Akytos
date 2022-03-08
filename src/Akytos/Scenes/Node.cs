using System.Collections.ObjectModel;

namespace Akytos;

/// <summary>
///     Represents an object or part of an object in the scene.
/// </summary>
public class Node
{
    [SerializeField("Children")]private readonly List<Node> m_children;
    [SerializeField("Name")]private string m_name;
    private SceneTree? m_sceneTree;

    public Node()
        : this("NewNode")
    {
    }
    
    /// <summary>
    ///     Creates a new node with the specified name.
    /// </summary>
    /// <param name="name"></param>
    public Node(string name)
    {
        m_name = name;

        m_children = new List<Node>();
    }

    public int Id => GetPath().GetHashCode();
    
    /// <summary>
    ///     Whether this Node is enabled in the Scene Hierarchy or not.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    ///     The parent node of this node. A null owner means that this is the root node of a <see cref="SceneTree" />
    /// </summary>
    public Node? Owner { get; private set; }

    /// <summary>
    ///     True if this node is currently inside a <see cref="SceneTree" />
    /// </summary>
    public bool IsInsideTree => SceneTree != null;

    /// <summary>
    ///     The <see cref="SceneTree" /> that this Node is currently a part of
    /// </summary>
    public SceneTree? SceneTree
    {
        get => m_sceneTree;
        internal set
        {
            bool treeExited = IsInsideTree && value == null;

            m_sceneTree = value;
            m_children.ForEach(n => n.SceneTree = value);
            if (value != null)
                TreeEntered?.Invoke();
            else if (treeExited) TreeExited?.Invoke();
        }
    }

    public ReadOnlyCollection<Node> ImmediateChildren => new(m_children);

    /// <summary>
    ///     The identifying name of this Node
    /// </summary>
    public string Name
    {
        get => m_name;
        set
        {
            m_name = value;
            Renamed?.Invoke();
        }
    }

    /// <summary>
    ///     Triggered when the Node is loaded in the game.
    /// </summary>
    public event Action? Ready;

    /// <summary>
    ///     Triggered when the Node enters a tree.
    /// </summary>
    public event Action? TreeEntered;

    /// <summary>
    ///     Triggered when the Node exits a tree.
    /// </summary>
    public event Action? TreeExited;

    /// <summary>
    ///     Triggered when the Node's name is changed.
    /// </summary>
    public event Action? Renamed;

    /// <summary>
    ///     Gets the current path of the Node.
    /// </summary>
    /// <returns>The Node's path. Null if the Node is not part of any <see cref="SceneTree" /></returns>
    public NodePath? GetPath()
    {
        if (!IsInsideTree)
        {
            Debug.LogError($"{Name} is not part of any {nameof(SceneTree)}.");
            return null;
        }

        string ownerPath = Owner?.GetPath() ?? "";

        string pathString = Owner != null ? $"{ownerPath}/{Name}" : $"/{Name}";

        return new NodePath(pathString);
    }

    /// <summary>
    ///     Finds a child Node specified by the path. Accepts absolute paths if the Node is inside a <see cref="SceneTree" />
    /// </summary>
    /// <param name="path">The path to search.</param>
    /// <param name="startIndex">Which index of the path to start searching.</param>
    /// <returns>The child Node if it exists.</returns>
    public Node? GetNode(NodePath path, int startIndex = 0)
    {
        if (path.IsEmpty) return this;

        string currentName = path.GetName(startIndex);

        if (currentName == Name && startIndex == path.GetConcatenatedNames().Split("/").Length - 1) return this;

        if (path.IsAbsolute && startIndex == 0)
        {
            var rootNode = SceneTree?.CurrentScene;
            if (rootNode == null || rootNode.Name != currentName) return null;

            return rootNode.GetNode(path, ++startIndex);
        }

        var node = m_children.Find(node => node.Name == currentName);

        if (node is null) return null;

        if (startIndex == path.GetConcatenatedNames().Split("/").Length - 1) return node;

        return node.GetNode(path, ++startIndex);
    }

    /// <summary>
    ///     Adds a Node as a child to this Node.
    /// </summary>
    /// <param name="node">The Node to add.</param>
    /// <param name="legibleUniqueName">Set to true to allow duplicate names. Name will be appended with a counter.</param>
    /// <returns>Ok if Node was added. InvalidData if Node already has an owner.</returns>
    public Result AddChild(Node node, bool legibleUniqueName = false)
    {
        if (node.Owner != null || node.Owner == null && node.SceneTree != null)
        {
            Debug.LogError($"{node.Name} already has an owner! Call 'RemoveChild(Node node)' first.");
            return Result.InvalidData;
        }

        int childrenWithMatchingNames = m_children.Count(n => n.Name == node.Name);

        if (childrenWithMatchingNames > 0)
        {
            if (legibleUniqueName)
            {
                node.Name = $"{node.Name}_{childrenWithMatchingNames}";
            }
            else
            {
                Debug.LogError($"There is already an immediate child named {node.Name}");
                return Result.InvalidData;
            }
        }

        m_children.Add(node);

        node.Owner = this;
        node.SceneTree = SceneTree;

        SceneTree?.OnNodeAdded(node);

        return Result.Ok;
    }

    /// <summary>
    ///     Removes the specified Node as a child.
    /// </summary>
    /// <param name="node">The Node to remove.</param>
    /// <returns>Ok if Node was removed. InvalidData if the Node was not a child.</returns>
    public Result RemoveChild(Node node)
    {
        if (node.Owner != this || !m_children.Remove(node))
        {
            Debug.LogError($"{node.Name} is not a child of {Name}.");
            return Result.InvalidData;
        }

        node.Owner = null;
        node.SceneTree = null;

        SceneTree?.OnNodeRemoved(node);

        return Result.Ok;
    }

    /// <summary>
    ///     Gets all children of this Node.
    /// </summary>
    /// <param name="includeRoot">Set to true to include the root in the list.</param>
    /// <param name="predicate">A boolean function predicate to filter out results.</param>
    /// <returns>A complete list of all children.</returns>
    public IEnumerable<Node> GetChildren(bool includeRoot = false, Func<Node, bool>? predicate = null)
    {
        var nodes = includeRoot ? new Stack<Node>(new[] {this}) : new Stack<Node>(m_children);
        while (nodes.Any())
        {
            var node = nodes.Pop();

            bool shouldRetrieveNode = true;
            if (predicate != null) shouldRetrieveNode = predicate(node);

            if (shouldRetrieveNode) yield return node;

            foreach (var n in node.m_children) nodes.Push(n);
        }
    }

    /// <summary>
    ///     Called when the Node is loaded.
    /// </summary>
    public virtual void OnReady()
    {
        Ready?.Invoke();
    }

    /// <summary>
    ///     Called once every frame.
    /// </summary>
    /// <param name="deltaSeconds">The elapsed seconds since the previous frame.</param>
    public virtual void OnUpdate(float deltaSeconds)
    {
    }
}