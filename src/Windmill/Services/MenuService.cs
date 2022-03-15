using Akytos;
using ImGuiNET;
using Windmill.Panels;

namespace Windmill.Services;

internal class MenuService
{
    private readonly PanelManager m_panelManager;
    private readonly SceneEditorContext m_editorContext;

    public MenuService(PanelManager panelManager, SceneEditorContext editorContext)
    {
        m_panelManager = panelManager;
        m_editorContext = editorContext;
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

    private void DrawFileMenu()
    {
        if (ImGui.BeginMenu("File"))
        {
            if (ImGui.MenuItem("New scene"))
            {
                m_editorContext.CreateNewScene<Node2D>();
            }

            if (ImGui.MenuItem("Load scene..."))
            {
                m_editorContext.LoadScene();
            }

            if (ImGui.MenuItem("Save scene"))
            {
                m_editorContext.SaveScene();
            }

            if (ImGui.MenuItem("Save scene as..."))
            {
                m_editorContext.SaveSceneAs();
            }

            if (ImGui.MenuItem("Exit"))
            {
                Application.Exit();
            }

            ImGui.EndMenu();
        }
    }
}