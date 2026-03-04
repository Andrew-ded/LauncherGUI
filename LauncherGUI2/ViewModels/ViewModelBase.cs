using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LauncherGUI.ViewModels;

/// <summary>
/// Базовый класс ViewModel с реализацией уведомлений об изменении свойств.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <summary>
    /// Событие вызывается при изменении значения свойства.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Уведомляет UI о том, что свойство изменилось.
    /// </summary>
    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
