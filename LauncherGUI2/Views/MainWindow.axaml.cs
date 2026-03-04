using Avalonia.Controls;
using Avalonia.Input;
using LauncherGUI.ViewModels;

namespace LauncherGUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TitleBar_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    private void AppCard_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Control control &&
            control.DataContext is AppCardViewModel card &&
            DataContext is MainWindowViewModel vm)
        {
            vm.LaunchApplicationCommand.Execute(card);
        }
    }

    private void Minimize_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void ToggleMaximize_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void Close_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}
