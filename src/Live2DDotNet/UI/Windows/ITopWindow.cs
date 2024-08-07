using Avalonia.Controls;
using Avalonia.Input;

namespace Live2DDotNet.UI.Windows;

public interface ITopWindow
{
    public void Opened();
    public void WindowStateChange(WindowState state);
    public Task<bool> OnKeyDown(object? sender, KeyEventArgs e);
}
