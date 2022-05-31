using Akytos;
using Akytos.Events;
using Akytos.SceneSystems;
using Windmill.Actions;
using Windmill.Modals;

namespace Windmill.Services;

internal class EditorHotKeyService
{
    private readonly SceneEditorContext m_sceneEditorContext;

    private bool m_isControlDown;
    private bool m_isShiftDown;
    private ModalStack m_modalStack;
    private ActionExecutor m_actionExecutor;

    public EditorHotKeyService(SceneEditorContext sceneEditorContext, ModalStack modalStack, ActionExecutor actionExecutor)
    {
        m_sceneEditorContext = sceneEditorContext;
        m_modalStack = modalStack;
        m_actionExecutor = actionExecutor;
    }

    public void OnEvent(IEvent e)
    {
        var dispatcher = new EventDispatcher(e);
        dispatcher.Dispatch<KeyDownEvent>(OnKeyDownEvent);
        dispatcher.Dispatch<KeyUpEvent>(OnKeyUpEvent);
    }

    private bool OnKeyDownEvent(KeyDownEvent e)
    {
        switch (e.KeyCode)
        {
            /*
             * MODIFIER KEYS
             */
            case KeyCode.ControlLeft:
            case KeyCode.ControlRight:
            {
                m_isControlDown = true;
                break;
            }
            case KeyCode.ShiftLeft:
            case KeyCode.ShiftRight:
            {
                m_isShiftDown = true;
                break;
            }
        }

        return false;
    }

    private bool OnKeyUpEvent(KeyUpEvent e)
    {
        switch (e.KeyCode)
        {
            /*
             * EDIT ACTIONS
             */
            case KeyCode.Z:
            {
                if (m_isControlDown)
                {
                    m_actionExecutor.Undo();
                    return true;
                }
                break;
            }
            case KeyCode.Y:
            {
                if (m_isControlDown)
                {
                    m_actionExecutor.Redo();
                    return true;
                }
                break;
            }
            /*
             *   NODE ACTIONS
             */
            case KeyCode.Delete:
            {
                if (m_sceneEditorContext.SelectedNode != null)
                {
                    var deleteNodeAction =
                        new DeleteNodeAction(m_sceneEditorContext, m_sceneEditorContext.SelectedNode);
                    
                    m_actionExecutor.Execute(deleteNodeAction);
                }
                
                return true;
            }
            /*
             *   SCENE ACTIONS
             */
            case KeyCode.N:
            {
                if (m_isControlDown)
                {
                    m_sceneEditorContext.CreateNewScene<Node2D>();
                    return true;
                }
                
                break;
            }
            case KeyCode.S:
            {
                if (m_isControlDown)
                {
                    if (m_isShiftDown || m_sceneEditorContext.CurrentSceneFilename == null)
                    {
                        m_modalStack.PushModal<SaveSceneModal>();
                    }
                    else
                    {
                        m_sceneEditorContext.SaveSceneAs(m_sceneEditorContext.CurrentSceneFilename);
                    }
                    return true;
                }
                
                break;
            }
            case KeyCode.O:
            {
                if (m_isControlDown)
                {
                    m_modalStack.PushModal<LoadSceneModal>();
                    return true;
                }
                
                break;
            }
            case KeyCode.X:
            {
                if (m_isControlDown)
                {
                    Application.Exit();
                    return true;
                }
                
                break;
            }
            /*
             * MODIFIER KEYS
             */
            case KeyCode.ControlLeft:
            case KeyCode.ControlRight:
            {
                m_isControlDown = false;
                break;
            }
            case KeyCode.ShiftLeft:
            case KeyCode.ShiftRight:
            {
                m_isShiftDown = false;
                break;
            }
        }

        return false;
    }
}