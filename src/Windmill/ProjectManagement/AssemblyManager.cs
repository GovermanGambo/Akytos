using System.IO;
using Akytos;
using Windmill.Utilities;

namespace Windmill.ProjectManagement;

internal class AssemblyManager
{
    private readonly IProjectManager m_projectManager;
    private readonly AssemblyContainer m_assemblyContainer;

    public AssemblyManager(IProjectManager projectManager, AssemblyContainer assemblyContainer)
    {
        m_projectManager = projectManager;
        m_assemblyContainer = assemblyContainer;
    }

    public void BuildAndLoadAssemblies()
    {
        m_assemblyContainer.UnloadExternalAssemblies();
        
        // Locate the project assembly directory and build the entire solution
        string assemblyDirectory =
            Path.Combine(Application.WorkingDirectory, SystemConstants.FileSystem.AssemblySubDirectory);
        DotnetUtility.BuildSolution(assemblyDirectory, BuildConfiguration.Debug);

        // Create and locate the project Library directory, and copy the assembly DLL to it
        string libraryPath = Path.Combine(Application.WorkingDirectory, SystemConstants.FileSystem.LibrarySubDirectory);
        if (!Directory.Exists(libraryPath))
        {
            Directory.CreateDirectory(libraryPath);
        }
        string projectName = m_projectManager.CurrentProject.ProjectName;
        string destination = Path.Combine(libraryPath, $"{projectName}.dll");
        string dllLocation = Path.Combine(assemblyDirectory, $"{projectName}", "bin", "Debug", "net6.0", $"{projectName}.dll");
        File.Copy(dllLocation, destination, true);
        
        m_assemblyContainer.LoadExternalAssemblies();
    }
}