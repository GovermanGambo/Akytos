using Akytos;
using ImGuiNET;
using Windmill.Modals;
using Windmill.Panels;

namespace Windmill.Services;

internal class MenuService
{
    private readonly PanelManager m_panelManager;
    private readonly SceneEditorContext m_editorContext;
    private readonly ModalStack m_modalStack;
    
    public MenuService(PanelManager panelManager, SceneEditorContext editorContext, ModalStack modalStack)
    {
        m_panelManager = panelManager;
        m_editorContext = editorContext;
        m_modalStack = modalStack;
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
                m_modalStack.PushModal<LoadSceneModal>();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem("Save scene"))
            {
                if (m_editorContext.CurrentSceneFilename == null)
                {
                    m_modalStack.PushModal<SaveSceneModal>();
                }
                else
                {
                    m_editorContext.SaveSceneAs(m_editorContext.CurrentSceneFilename);
                }
            }

            if (ImGui.MenuItem("Save scene as..."))
            {
                m_modalStack.PushModal<SaveSceneModal>();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem("Exit"))
            {
                Application.Exit();
            }

            ImGui.EndMenu();
        }
    }
}