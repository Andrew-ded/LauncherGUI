using System;
using System.Windows.Input;

namespace LauncherGUI.ViewModels;

/// <summary>
/// Универсальная команда для привязок в XAML.
/// </summary>
public class RelayCommand : ICommand
{
    /// <summary>Действие, которое выполняется при вызове команды.</summary>
    private readonly Action<object?> _execute;

    /// <summary>Проверка, можно ли выполнить команду в текущем состоянии.</summary>
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>
    /// Создает команду.
    /// </summary>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <summary>
    /// Событие для обновления состояния доступности команды.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Возвращает, можно ли выполнить команду.
    /// </summary>
    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    /// <summary>
    /// Выполняет команду.
    /// </summary>
    public void Execute(object? parameter) => _execute(parameter);

    /// <summary>
    /// Принудительно уведомляет UI о смене состояния <see cref="CanExecute(object?)"/>.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
