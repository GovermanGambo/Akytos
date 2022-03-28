using System;
using Akytos;
using LightInject;

namespace Windmill;

public class WindmillApp : Application
{
    public WindmillApp(string title, int initialWindowWidth, int initialWindowHeight) : base(title, initialWindowWidth, initialWindowHeight)
    {
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        
        PushLayer<EditorLayer>();
    }

    protected override void OnRestart()
    {
        // Starts a new instance of the program itself
        System.Diagnostics.Process.Start("Windmill.exe");
    }

    protected override void RegisterServices(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterFrom<EditorCompositionRoot>();
    }
}