using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Controls;
using ColorMC.Gui.UI.Controls.Error;
using ColorMC.Gui.UI.Controls.Main;
using ColorMC.Gui.UI.Controls.Setting;
using ColorMC.Gui.UI.Model;
using ColorMC.Gui.UI.Model.Items;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.UIBinding;
using ColorMC.Gui.Utils;

namespace ColorMC.Gui.Manager;

public static class WindowManager
{
    public static Window? LastWindow { get; set; }

    public static SingleControl? AllWindow { get; set; }
    public static MainControl? MainWindow { get; set; }
    public static DllAssembly? CustomWindow { get; set; }
    public static SettingControl? SettingWindow { get; set; }

    private static readonly WindowTransparencyLevel[] WindowTran =
    [
        WindowTransparencyLevel.None,
        WindowTransparencyLevel.Transparent,
        WindowTransparencyLevel.Blur,
        WindowTransparencyLevel.AcrylicBlur,
        WindowTransparencyLevel.Mica
    ];

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

    public static void AWindow(BaseUserControl con, bool newwindow = false)
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
            if (CustomWindow == null)
            {
                return null;
            }
            else
            {
                return CustomWindow.Window.Window;
            }
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
        else if (CustomWindow?.Window.GetVisualRoot() is Window window1)
        {
            window1.Show();
            window1.WindowState = WindowState.Normal;
            window1.Activate();
        }
    }

    public static void Hide()
    {
        if (MainWindow?.GetVisualRoot() is Window window)
        {
            window.Hide();
            (CustomWindow?.Window.GetVisualRoot() as Window)?.Close();
        }
        else if (CustomWindow?.Window.GetVisualRoot() is Window window1)
        {
            window1.Hide();
        }
        CloseAllWindow();
    }

    public static void CloseAllWindow()
    {
    }
}
