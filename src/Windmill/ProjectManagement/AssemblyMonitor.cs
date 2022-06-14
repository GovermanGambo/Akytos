using System;
using System.IO;
using Akytos;

namespace Windmill.ProjectManagement;

internal class AssemblyMonitor : IDisposable
{
    private const float IntervalSeconds = 1.0f;
    
    private readonly FileSystemWatcher m_fileSystemWatcher;
    private readonly AssemblyManager m_assemblyManager;

    private float m_lastTime;
    private bool m_didChange;

    public AssemblyMonitor(AssemblyManager assemblyManager)
    {
        m_assemblyManager = assemblyManager;
        string assemblyDirectory =
            Path.Combine(Application.WorkingDirectory, SystemConstants.FileSystem.AssemblySubDirectory);
        m_fileSystemWatcher = new FileSystemWatcher(assemblyDirectory);
        m_fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
    }

    public void Tick(DeltaTime time)
    {
        if (m_lastTime > IntervalSeconds && m_didChange)
        {
            m_assemblyManager.BuildAndLoadAssemblies();
            m_lastTime = 0f;
            m_didChange = false;
        }

        m_lastTime += time;
    }

    public void Dispose()
    {
        m_fileSystemWatcher.Dispose();
    }
    
    private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
    {
        m_didChange = true;
    }
}