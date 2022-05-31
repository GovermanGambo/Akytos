using Akytos;
using Akytos.SceneSystems;
using ImGuiNET;
using Windmill.Actions;
using Windmill.Modals;
using Windmill.Panels;
using Windmill.ProjectManagement;
using Windmill.Resources;

namespace Windmill.Services;

internal class MenuService
{
    private readonly PanelManager m_panelManager;
    private readonly SceneEditorContext m_editorContext;
    private readonly ModalStack m_modalStack;
    private readonly ActionExecutor m_actionExecutor;
    private readonly AssemblyManager m_assemblyManager;
    
    public MenuService(PanelManager panelManager, SceneEditorContext editorContext, ModalStack modalStack, ActionExecutor actionExecutor, AssemblyManager assemblyManager)
    {
        m_panelManager = panelManager;
        m_editorContext = editorContext;
        m_modalStack = modalStack;
        m_actionExecutor = actionExecutor;
        m_assemblyManager = assemblyManager;
    }

    public void OnDrawGui()
    {
        if (ImGui.BeginMenuBar())
        {
            DrawFileMenu();

            if (ImGui.BeginMenu(LocalizedStrings.Edit))
            {
                if (ImGui.MenuItem(LocalizedStrings.Undo, "Ctrl+Z", false, m_actionExecutor.CanUndo))
                {
                    m_actionExecutor.Undo();
                }

                if (ImGui.MenuItem(LocalizedStrings.Redo, "Ctrl+Y", false, m_actionExecutor.CanRedo))
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
        if (ImGui.BeginMenu(LocalizedStrings.File))
        {
            if (ImGui.MenuItem(LocalizedStrings.NewScene, "Ctrl+N"))
            {
                m_editorContext.CreateNewScene<Node2D>();
            }

            if (ImGui.MenuItem(LocalizedStrings.OpenScene, "Ctrl+O"))
            {
                m_modalStack.PushModal<LoadSceneModal>();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem(LocalizedStrings.SaveScene, "Ctrl+S"))
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

            if (ImGui.MenuItem(LocalizedStrings.SaveSceneAs, "Ctrl+Shift+S"))
            {
                m_modalStack.PushModal<SaveSceneModal>();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem(LocalizedStrings.ProjectManager))
            {
                m_modalStack.PushModal<ProjectManagerModal>();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem("Build assemblies"))
            {
                m_assemblyManager.BuildAndLoadAssemblies();
            }
            
            ImGui.Separator();

            if (ImGui.MenuItem(LocalizedStrings.Exit, "Ctrl+X"))
            {
                Application.Exit();
            }

            ImGui.EndMenu();
        }
    }
}