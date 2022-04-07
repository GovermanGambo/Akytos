using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Akytos;
using Akytos.Diagnostics.Logging;

namespace Windmill.ProjectManagement;

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
                Log.Core.Error(e, "Failed to load assembly {0}", assembly);
            }
        }
    }
    
    private IEnumerable<string> GetAssemblyFiles()
    {
        string projectName = m_projectManager.CurrentProject.ProjectName;
        string assembly = Path.Combine(Application.WorkingDirectory,
            SystemConstants.FileSystem.AssemblySubDirectory,
            projectName, "bin", "Release", "net6.0", $"{projectName}.dll");

        return new[] { assembly };
    }
}