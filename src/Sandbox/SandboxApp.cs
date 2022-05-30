using Akytos;
using Akytos.Configuration;
using Akytos.Graphics;
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

    protected override void Configure(IAppConfigurator configurator)
    {
        configurator.ConfigureLayers(layers => layers.AddLayer<SandboxLayer>());
        configurator.ConfigureGame(game => game.SetWindowTitle("Akytos Sandbox"));
    }

    protected override void RegisterServices(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.AddGraphics(GraphicsBackend.OpenGL);
        serviceRegistry.RegisterSingleton(_ => m_akytosProject);
    }

    protected override void OnRestart()
    {
    }
}