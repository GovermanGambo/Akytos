using System.Diagnostics;

namespace Akytos.Utilities;

public class SystemCommand
{
    private readonly ProcessStartInfo m_processStartInfo;

    public SystemCommand(string commandString)
    {
        m_processStartInfo = new ProcessStartInfo(commandString);
    }

    public void Run(bool waitForCompletion = true)
    {
        var process = Process.Start(m_processStartInfo);
        if (process == null)
        {
            throw new Exception($"Failed to start process {m_processStartInfo.FileName}");
        }

        if (waitForCompletion)
        {
            process.WaitForExit();
        }
    }

    public void AddArgument(string argument)
    {
        m_processStartInfo.Arguments += $" {argument}";
    }

    public void SetWorkingDirectory(string workingDirectory)
    {
        m_processStartInfo.WorkingDirectory = workingDirectory;
    }
}