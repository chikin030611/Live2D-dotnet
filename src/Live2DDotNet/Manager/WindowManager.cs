using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Live2DDotNet.UI.Controls.Error;
using Live2DDotNet.UI.Controls.Main;
using Live2DDotNet.UI.Model;
using Live2DDotNet.UI.Windows;
using Live2DDotNet.Objs;
using Live2DDotNet.UI.Controls;
using Live2DDotNet.UI.Controls.Setting;

namespace Live2DDotNet.Manager;

public static class WindowManager
{
    public static Window? LastWindow { get; set; }

    public static SingleControl? AllWindow { get; set; }
    public static MainControl? MainWindow { get; set; }
    public static SettingControl? SettingWindow { get; set; }

    public static void Init()
    {
        ShowMain();
    }

    public static IBaseWindow FindRoot(object? con)
    {
        if (con is SingleControl all)
            return all;
        else if (con is IBaseWindow win)
            return win;
        else if (con is BaseUserControl con1)
            return con1.Window;

        return AllWindow!;
    }

    public static void AWindow(BaseUserControl con)
    {
        AMultiWindow win;
        win = new MultiWindow(con);
        App.TopLevel ??= win;
        con.SetBaseModel(win.Model);
        win.Show();
    }

    public static void ShowMain()
    {
        if (MainWindow != null)
        {
            MainWindow.Window.TopActivate();
        }
        else
        {
            MainWindow = new();
            AWindow(MainWindow);
        }
    }

    public static void ShowSetting(SettingType type, int value = 0)
    {
        if (SettingWindow != null)
        {
            SettingWindow.Window.TopActivate();
        }
        else
        {
            SettingWindow = new(value);
            AWindow(SettingWindow);
        }

        SettingWindow?.GoTo(type);
    }

    public static void ShowError(string? data, Exception? e, bool close = false)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var con = new ErrorControl(data, e, close);
            AWindow(con);
        });
    }

    public static void ShowError(string data, string e, bool close = false)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var con = new ErrorControl(data, e, close);
            AWindow(con);
        });
    }

    public static IBaseWindow? GetMainWindow()
    {
        if (MainWindow == null)
        {
            return null;
        }

        return MainWindow.Window;
    }

    public static void UpdateWindow(BaseModel model)
    {
        model.BgVisible = false;

        model.Hints = [WindowTransparencyLevel.None];

        model.Theme = ThemeManager.NowTheme ==
            PlatformThemeVariant.Light ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    public static void Show()
    {
        if (MainWindow?.GetVisualRoot() is Window window)
        {
            window.Show();
            window.WindowState = WindowState.Normal;
            window.Activate();
        }
    }

    public static void Hide()
    {
        if (MainWindow?.GetVisualRoot() is Window window)
        {
            window.Hide();
        }
        CloseAllWindow();
    }

    public static void CloseAllWindow()
    {
    }
}
