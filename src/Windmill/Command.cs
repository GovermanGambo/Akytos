using System;

namespace Windmill;

public class Command : ICommand
{
    private readonly Action<object> m_command;
    private readonly Func<bool>? m_canExecute;

    public Command(Action<object> command, Func<bool>? canExecute = null)
    {
        m_command = command;
        m_canExecute = canExecute;
    }
    
    public void Execute(object commandParameter)
    {
        if (!CanExecute())
        {
            return;
        }

        m_command(commandParameter);
    }

    public bool CanExecute()
    {
        return m_canExecute is null || m_canExecute();
    }
}

public class Command<T> : ICommand
{
    private readonly Action<T> m_command;
    private readonly Func<bool>? m_canExecute;

    public Command(Action<T> command, Func<bool>? canExecute = null)
    {
        m_command = command;
        m_canExecute = canExecute;
    }
    
    public void Execute(T commandParameter)
    {
        if (!CanExecute())
        {
            return;
        }

        m_command(commandParameter);
    }

    public void Execute(object commandParameter)
    {
        Execute((T)commandParameter);
    }

    public bool CanExecute()
    {
        return m_canExecute is null || m_canExecute();
    }
}

public interface ICommand
{
    void Execute(object commandParameter);
    bool CanExecute();
}