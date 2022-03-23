using Akytos;
using ImGuiNET;
using Windmill.Actions;
using Windmill.Modals;
using Windmill.Panels;

namespace Windmill.Services;

internal class MenuService
{
    private readonly PanelManager m_panelManager;
    private readonly SceneEditorContext m_editorContext;
    private readonly ModalStack m_modalStack;
    private readonly ActionExecutor m_actionExecutor;
    
    public MenuService(PanelManager panelManager, SceneEditorContext editorContext, ModalStack modalStack, ActionExecutor actionExecutor)
    {
        m_panelManager = panelManager;
        m_editorContext = editorContext;
        m_modalStack = modalStack;
        m_actionExecutor = actionExecutor;
    }

    public void OnDrawGui()
    {
        if (ImGui.BeginMenuBar())
        {
            DrawFileMenu();

            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Undo", m_actionExecutor.CanUndo))
                {
                    m_actionExecutor.Undo();
                }

                if (ImGui.MenuItem("Redo", m_actionExecutor.CanRedo))
                {
                    m_actionExecutor.Redo();
                }
            }

            if (ImGui.BeginMenu("Layout"))
            {
                foreach (var panel in m_panelManager)
                {
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
            if (ImGui.MenuItem("New scene", "Ctrl+N"))
            {
                m_editorContext.CreateNewScene<Node2D>();
            }

            if (ImGui.MenuItem("Open scene...", "Ctrl+O"))
            {
                m_modalStack.PushModal<LoadSceneModal>();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem("Save scene", "Ctrl+S"))
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

            if (ImGui.MenuItem("Save scene as...", "Ctrl+Shift+S"))
            {
                m_modalStack.PushModal<SaveSceneModal>();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem("Exit", "Ctrl+X"))
            {
                Application.Exit();
            }

            ImGui.EndMenu();
        }
    }
}