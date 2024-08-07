using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ColorMC.Gui.Objs;
using ColorMC.Gui.Utils;
using ColorMC.Gui.Manager;
using ColorMC.Gui.Objs;

namespace ColorMC.Gui.UI.Windows;

public abstract class ABaseWindow : Window
{
    public abstract ITopWindow ICon { get; }

    protected void InitBaseWindow()
    {

        AddHandler(KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);

        Opened += UserWindow_Opened;
        PropertyChanged += OnPropertyChanged;
    }

    private void UserWindow_Opened(object? sender, EventArgs e)
    {
        ICon.Opened();
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == WindowStateProperty)
        {
            ICon.WindowStateChange(WindowState);
            if (WindowState == WindowState.Maximized)
            {
                Padding = new Thickness(8);
            }
            else
            {
                Padding = new Thickness(0);
            }
        }
    }

    private async void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (await ICon.OnKeyDown(sender, e))
        {
            e.Handled = true;
            return;
        }
    }
}
