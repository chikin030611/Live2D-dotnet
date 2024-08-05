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
        if (ConfigBinding.WindowMode())
        {
            if (SystemInfo.Os == OsType.Android)
            {
                AllWindow = new();
                AllWindow.Model.HeadDisplay = false;
                AllWindow.Opened();
            }
            else
            {
                var win = new SingleWindow();
                AllWindow = win.Win;
                win.Show();
            }

            Dispatcher.UIThread.Post(() =>
            {
                App.TopLevel ??= TopLevel.GetTopLevel(AllWindow);
            });
        }

        if (!ShowCustom())
        {
            ShowMain();
        }
    }

    public static IBaseWindow FindRoot(object? con)
    {
        if (con is SingleControl all)
            return all;
        else if (GuiConfigUtils.Config.WindowMode)
            return AllWindow!;
        else if (con is IBaseWindow win)
            return win;
        else if (con is BaseUserControl con1)
            return con1.Window;

        return AllWindow!;
    }

    public static bool ShowCustom(bool test = false)
    {
        if (CustomWindow != null)
        {
            CustomWindow.Window.Window.TopActivate();
            return true;
        }

        if (!test)
        {
            var config = GuiConfigUtils.Config.ServerCustom;
            if (config == null || config?.EnableUI == false)
            {
                return false;
            }
        }

        try
        {
            string file = BaseBinding.GetRunDir() + "ColorMC.CustomGui.dll";
            if (!File.Exists(file))
            {
                return false;
            }

            var dll = new DllAssembly();

            if (dll.IsLoad)
            {
                if (!test)
                {
                    CustomWindow = dll;
                }
                AWindow(dll.Window, test);
                var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/ColorMC/custom";
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "custom");
                    CustomWindow?.Window.Window.Model.Show(App.Lang("WindowManager.Info1"));
                }
            }
            return true;
        }
        catch (Exception e)
        {
            var data = App.Lang("WindowManager.Error1");
            Logs.Error(data, e);
            ShowError(data, e, true);
        }

        return false;
    }

    public static void AWindow(BaseUserControl con, bool newwindow = false)
    {
        if (ConfigBinding.WindowMode())
        {
            if (newwindow)
            {
                if (SystemInfo.Os == OsType.Android)
                {
                    return;
                }

                AMultiWindow win;
                win = new MultiWindow(con);
                con.SetBaseModel(win.Model);
                win.Show();
            }
            else
            {
                con.SetBaseModel(AllWindow!.Model);
                AllWindow.Add(con);
            }
        }
        else
        {
            AMultiWindow win;
            win = new MultiWindow(con);
            App.TopLevel ??= win;
            con.SetBaseModel(win.Model);
            win.Show();
        }
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
        if (ConfigBinding.WindowMode())
        {
            return AllWindow;
        }
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
        model.Back = ImageManager.BackBitmap;
        if (ImageManager.BackBitmap != null)
        {
            if (GuiConfigUtils.Config.BackTran != 0)
            {
                model.BgOpacity = (double)(100 - GuiConfigUtils.Config.BackTran) / 100;
            }
            else
            {
                model.BgOpacity = 1.0;
            }
            model.BgVisible = true;
        }
        else
        {
            model.BgVisible = false;
        }

        if (GuiConfigUtils.Config.WindowTran)
        {
            model.Hints = [WindowTran[GuiConfigUtils.Config.WindowTranType]];
        }
        else
        {
            model.Hints = [WindowTransparencyLevel.None];
        }

        model.Theme = ThemeManager.NowTheme ==
            PlatformThemeVariant.Light ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    public static void Show()
    {
        if (ConfigBinding.WindowMode())
        {
            if (AllWindow?.GetVisualRoot() is Window window)
            {
                window.Show();
                window.WindowState = WindowState.Normal;
                window.Activate();
            }
        }
        else
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
    }

    public static void Hide()
    {
        if (ConfigBinding.WindowMode())
        {
            if (AllWindow?.GetVisualRoot() is Window window)
            {
                window.Hide();
            }
        }
        else
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
    }

    public static void CloseAllWindow()
    {
    }
}
