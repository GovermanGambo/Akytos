using Akytos;
using ImGuiNET;
using Windmill.Panels;

namespace Windmill.Services;

internal class MenuService
{
    private readonly PanelManager m_panelManager;

    public MenuService(PanelManager panelManager)
    {
        m_panelManager = panelManager;
    }

    public void OnDrawGui()
    {
        if (ImGui.BeginMenuBar())
        {
            DrawFileMenu();

            if (ImGui.BeginMenu("Layout"))
            {
                foreach (var panel in m_panelManager)
                {
                    if (panel.HideInMenu)
                    {
                        continue;
                    }
                    
                    if (ImGui.MenuItem(panel.DisplayName, "", panel.IsEnabled))
                    {
                        panel.IsEnabled = !panel.IsEnabled;
                    }
                }
                ImGui.EndMenu();
            }
            ImGui.EndMenuBar();
        }
    }

    private static void DrawFileMenu()
    {
        if (ImGui.BeginMenu("File"))
        {
            if (ImGui.MenuItem("New scene"))
            {
            }

            if (ImGui.MenuItem("Load scene..."))
            {
            }

            if (ImGui.MenuItem("Save scene"))
            {
            }

            if (ImGui.MenuItem("Save scene as..."))
            {
            }

            if (ImGui.MenuItem("Exit"))
            {
                Application.Exit();
            }

            ImGui.EndMenu();
        }
    }
}