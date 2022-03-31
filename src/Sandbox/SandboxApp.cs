using Akytos;
using LightInject;

namespace Sandbox;

internal class SandboxApp : Application
{
    public SandboxApp(AkytosProject akytosProject) : base("Akytos Sandbox", 1280, 720)
    {
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
        serviceRegistry.RegisterSingleton<AkytosProject>();
    }

    protected override void OnRestart()
    {
    }
}