using Akytos;
using Akytos.SceneSystems;
using ImGuiNET;
using Windmill.Actions;
using Windmill.Modals;
using Windmill.Panels;
using Windmill.ProjectManagement;
using Windmill.Resources;
using Windmill.Runtime;

namespace Windmill.Services;

internal class MenuService
{
    private readonly PanelManager m_panelManager;
    private readonly SceneEditorContext m_editorContext;
    private readonly ModalStack m_modalStack;
    private readonly ActionExecutor m_actionExecutor;
    private readonly AssemblyManager m_assemblyManager;
    private readonly RuntimeManager m_runtimeManager;

    public MenuService(PanelManager panelManager, SceneEditorContext editorContext, ModalStack modalStack, ActionExecutor actionExecutor, AssemblyManager assemblyManager, RuntimeManager runtimeManager)
    {
        m_panelManager = panelManager;
        m_editorContext = editorContext;
        m_modalStack = modalStack;
        m_actionExecutor = actionExecutor;
        m_assemblyManager = assemblyManager;
        m_runtimeManager = runtimeManager;
    }

    public void OnDrawGui()
    {
        bool isGameRunning = m_runtimeManager.IsGameRunning;
        
        if (ImGui.BeginMenuBar())
        {
            DrawFileMenu();

            if (ImGui.BeginMenu(LocalizedStrings.Edit))
            {
                if (ImGui.MenuItem(LocalizedStrings.Undo, "Ctrl+Z", false, m_actionExecutor.CanUndo && !isGameRunning))
                {
                    m_actionExecutor.Undo();
                }

                if (ImGui.MenuItem(LocalizedStrings.Redo, "Ctrl+Y", false, m_actionExecutor.CanRedo && !isGameRunning))
                {
                    m_actionExecutor.Redo();
                }
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu(LocalizedStrings.Layout))
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
        bool isGameRunning = m_runtimeManager.IsGameRunning;
        if (ImGui.BeginMenu(LocalizedStrings.File))
        {
            if (ImGui.MenuItem(LocalizedStrings.NewScene, "Ctrl+N", false, !isGameRunning))
            {
                m_editorContext.CreateNewScene<Node2D>();
            }

            if (ImGui.MenuItem(LocalizedStrings.OpenScene, "Ctrl+O", false, !isGameRunning))
            {
                m_modalStack.PushModal<LoadSceneModal>();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem(LocalizedStrings.SaveScene, "Ctrl+S", false, !isGameRunning))
            {
                if (m_editorContext.CurrentSceneFilePath == null)
                {
                    m_modalStack.PushModal<SaveSceneModal>();
                }
                else
                {
                    m_editorContext.SaveSceneAs(m_editorContext.CurrentSceneFilePath);
                }
            }

            if (ImGui.MenuItem(LocalizedStrings.SaveSceneAs, "Ctrl+Shift+S", false, !isGameRunning))
            {
                m_modalStack.PushModal<SaveSceneModal>();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem(LocalizedStrings.ProjectManager, !isGameRunning))
            {
                m_modalStack.PushModal<ProjectManagerModal>();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem("Build assemblies", !isGameRunning))
            {
                m_assemblyManager.BuildAndLoadAssemblies();
            }

            if (ImGui.MenuItem(isGameRunning ? LocalizedStrings.StopGame : LocalizedStrings.RunGame))
            {
                if (isGameRunning)
                {
                    m_runtimeManager.StopGame();
                }
                else
                {
                    m_runtimeManager.StartGame();
                }
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem(LocalizedStrings.Exit, "Ctrl+X", !isGameRunning))
            {
                Application.Exit();
            }

            ImGui.EndMenu();
        }
    }
}