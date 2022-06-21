using System;

namespace Windmill.Panels;

public struct PanelSummary
{
    public PanelSummary(string id, string displayName, Type type)
    {
        Id = id;
        DisplayName = displayName;
        Type = type;
    }

    public string Id { get; }
    public string DisplayName { get; }
    public Type Type { get; }
}