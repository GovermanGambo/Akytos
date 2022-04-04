using Akytos;
using LightInject;

namespace Sandbox;

internal class SandboxApp : Application
{
    private readonly AkytosProject m_akytosProject;
    
    public SandboxApp(AkytosProject akytosProject)
    {
        m_akytosProject = akytosProject;
        WorkingDirectory = akytosProject.ProjectDirectory;
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        PushLayer<SandboxLayer>();
    }

    protected override void RegisterServices(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<SandboxLayer>();
        serviceRegistry.RegisterSingleton(_ => m_akytosProject);
    }

    protected override void OnRestart()
    {
    }
}