using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Akytos;
using Akytos.Diagnostics.Logging;
using Windmill.Utilities;

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
    
    public void UnloadAssemblies()
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

    public void BuildAndLoadAssemblies()
    {
        // TODO: This won't work because MSBuild can't overwrite the DLLs while the app is running. Should copy the DLL to another directory instead
        var assemblyDirectory =
            Path.Combine(Application.WorkingDirectory, SystemConstants.FileSystem.AssemblySubDirectory);
        
        //DotnetUtility.BuildSolution(assemblyDirectory, BuildConfiguration.Debug);
        
        LoadAssemblies();
    }
    
    private IEnumerable<string> GetAssemblyFiles()
    {
        string projectName = m_projectManager.CurrentProject.ProjectName;
        string assembly = Path.Combine(Application.WorkingDirectory,
            SystemConstants.FileSystem.AssemblySubDirectory,
            projectName, "bin", "Debug", "net6.0", $"{projectName}.dll");

        return new[] { assembly };
    }
}