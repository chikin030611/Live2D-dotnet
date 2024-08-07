using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Live2DDotNet.Manager;
using Live2DDotNet.UI.Model;
using Live2DDotNet.UIBinding;
using Live2DDotNet.UI.Controls;

namespace Live2DDotNet.UI.Windows;

public interface IBaseWindow
{
    public BaseModel Model { get; }
    public BaseUserControl ICon { get; }
    public void SetTitle(string data);
    public void SetIcon(Bitmap icon);
    public void Close()
    {
        if (this is Window window)
        {
            window.Close();
        }
    }

    virtual public void Show()
    {
        if (this is Window window)
        {
            window.Show();
        }
    }

    virtual public void TopActivate()
    {
        if (this is Window window)
        {
            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }
            window.Show();
            window.Activate();
        }
    }

    virtual public void Hide()
    {
        if (this is Window window)
        {
            window.Hide();
        }
    }

    void SetSize(int width, int height);
}