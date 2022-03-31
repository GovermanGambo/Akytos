// See https://aka.ms/new-console-template for more information

using Akytos;

namespace Sandbox;

public static class Program
{
    public static void Main(string[] args)
    {
        string projectDirectory = args.Length > 0 ? args[0] : "<none>";

        var project = AkytosProject.Load(projectDirectory);

        var sandboxApp = new SandboxApp(project);
        sandboxApp.Run();
    }
}