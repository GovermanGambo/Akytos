using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Akytos;
using Akytos.Diagnostics.Logging;

namespace Windmill.ProjectManagement;

internal class AssemblyContainer : IDisposable
{
    private readonly AssemblyLoadContext m_defaultLoadContext;
    private readonly IProjectManager m_projectManager;
    private AssemblyLoadContext m_assemblyLoadContext;

    public AssemblyContainer(IProjectManager projectManager)
    {
        m_projectManager = projectManager;
        m_defaultLoadContext = AssemblyLoadContext.Default;
        m_assemblyLoadContext = new AssemblyLoadContext("Scripts", true);

        m_projectManager.ProjectChanged += ProjectManager_OnProjectChanged;
    }

    public void Dispose()
    {
        m_assemblyLoadContext.Unload();
        m_projectManager.ProjectChanged -= ProjectManager_OnProjectChanged;
    }

    public void LoadExternalAssemblies()
    {
        var assemblies = GetAssemblyFiles();

        foreach (var assembly in assemblies)
            try
            {
                using var fs = File.OpenRead(assembly);

                m_assemblyLoadContext.LoadFromStream(fs);
            }
            catch (Exception e)
            {
                Log.Core.Error(e, "Failed to load assembly {0}", assembly);
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
    
    private void ProjectManager_OnProjectChanged()
    {
        UnloadExternalAssemblies();
        LoadExternalAssemblies();
    }

    private IEnumerable<string> GetAssemblyFiles()
    {
        // TODO: Have some way to define assemblies which can be loaded here. That way, we have support for multiple assemblies per project
        var projectName = m_projectManager.CurrentProject.ProjectName;
        var assembly = Path.Combine(Application.WorkingDirectory,
            SystemConstants.FileSystem.LibrarySubDirectory, $"{projectName}.dll");

        return new[] { assembly };
    }
}