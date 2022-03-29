using System.Reflection;

namespace Akytos.ProjectManagement;

internal class AssemblyManager
{
    private readonly IProjectManager m_projectManager;

    public AssemblyManager(IProjectManager projectManager)
    {
        m_projectManager = projectManager;
    }

    public void LoadAssemblies()
    {
        var assemblies = GetAssemblyFiles();

        foreach (var assembly in assemblies)
        {
            try
            {
                Assembly.LoadFrom(assembly);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load assembly {0}", assembly);
            }
        }
    }
    
    private IEnumerable<string> GetAssemblyFiles()
    {
        string projectName = m_projectManager.CurrentProject.ProjectName;
        string assembly = Path.Combine(AkytosProject.CurrentWorkingDirectory,
            SystemConstants.FileSystem.AssemblySubDirectory,
            projectName, "bin", "Release", "net6.0", $"{projectName}.dll");

        return new[] { assembly };
    }
}