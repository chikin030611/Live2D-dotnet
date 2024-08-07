using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Live2DDotNet.Objs;
using Live2DDotNet.UIBinding;
using Live2DDotNet.Manager;
using Live2DDotNet.Utils;

namespace Live2DDotNet;

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
            setting.ColorValuesChanged += (sender, e) =>
            {
                ThemeManager.Init();
            };
        }

        BaseBinding.Init();

        ThemeManager.Init();

        WindowManager.Init();

        if (Live2DDotNetGui.RunType != RunType.AppBuilder)
        {
            Task.Run(() =>
            {
                Live2DDotNetCore.Init1();
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
        Live2DDotNetCore.Close();
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
