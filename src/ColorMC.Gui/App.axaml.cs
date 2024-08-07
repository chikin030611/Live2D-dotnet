using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using ColorMC.Core;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Manager;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UIBinding;
using ColorMC.Gui.Utils;

namespace ColorMC.Gui;

public partial class App : Application
{
    public App()
    {
        ThisApp = this;

        AppDomain.CurrentDomain.UnhandledException += (a, e) =>
        {
            string temp = "Thread Error";
            Logs.Error(temp, e.ExceptionObject as Exception);
            WindowManager.ShowError(temp, e.ExceptionObject as Exception);
        };
    }

    public static TopLevel? TopLevel { get; set; }

    public static event Action? OnClose;

    public static Application ThisApp { get; private set; }
    public static IApplicationLifetime? Life { get; private set; }

    public static bool IsHide { get; private set; }

    private static readonly Language s_language = new();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        Life = ApplicationLifetime;

        if (PlatformSettings is { } setting)
        {
            setting.ColorValuesChanged += (object? sender, PlatformColorValues e) =>
            {
                ThemeManager.Init();
            };
        }

        BaseBinding.Init();

        ThemeManager.Init();

        WindowManager.Init();

        if (ColorMCGui.RunType != RunType.AppBuilder)
        {
            Task.Run(() =>
            {
                ColorMCCore.Init1();
            });
        }
    }

    public static void Clear()
    {
        ThemeManager.Remove();
    }

    public static void Close()
    {
        OnClose?.Invoke();
        WindowManager.CloseAllWindow();
        ColorMCCore.Close();
        (Life as IClassicDesktopStyleApplicationLifetime)?.Shutdown();
        Environment.Exit(Environment.ExitCode);
    }

    public static void Show()
    {
        IsHide = false;
        Dispatcher.UIThread.Post(WindowManager.Show);
    }

    public static void Hide()
    {
        IsHide = true;
        WindowManager.Hide();
    }

    public static void TestClose()
    {
    }
}
