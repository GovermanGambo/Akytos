using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Akytos;
using Akytos.Diagnostics.Logging;

namespace Windmill.ProjectManagement;

internal class AssemblyContainer
{
    private readonly AssemblyLoadContext m_defaultLoadContext;
    private AssemblyLoadContext m_assemblyLoadContext;
    private readonly IProjectManager m_projectManager;

    public AssemblyContainer(IProjectManager projectManager)
    {
        m_projectManager = projectManager;
        m_defaultLoadContext = AssemblyLoadContext.Default;
        m_assemblyLoadContext = new AssemblyLoadContext("Scripts", true);
    }

    public void LoadExternalAssemblies()
    {
        var assemblies = GetAssemblyFiles();

        foreach (string assembly in assemblies)
        {
            try
            {
                m_assemblyLoadContext.LoadFromAssemblyPath(assembly);
            }
            catch (Exception e)
            {
                Log.Core.Error(e, "Failed to load assembly {0}", assembly);
            }
        }
    }

    public void UnloadExternalAssemblies()
    {
        m_assemblyLoadContext.Unload();

        m_assemblyLoadContext = new AssemblyLoadContext("Scripts", true);
    }

    public IEnumerable<Assembly> GetAssemblies()
    {
        return m_defaultLoadContext.Assemblies.Concat(m_assemblyLoadContext.Assemblies);
    }
    
    private IEnumerable<string> GetAssemblyFiles()
    {
        // TODO: Have some way to define assemblies which can be loaded here. That way, we have support for multiple assemblies per project
        string projectName = m_projectManager.CurrentProject.ProjectName;
        string assembly = Path.Combine(Application.WorkingDirectory,
            SystemConstants.FileSystem.LibrarySubDirectory, $"{projectName}.dll");

        return new[] {assembly};
    }
}