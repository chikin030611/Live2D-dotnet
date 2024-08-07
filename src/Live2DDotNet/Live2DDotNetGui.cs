using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Net;
using System.Text;
using Avalonia;
using Avalonia.Media;
using Tmds.DBus.Protocol;
using Live2DDotNet.Objs;
using Live2DDotNet.Utils;
using Live2DDotNet.UIBinding;

namespace Live2DDotNet;

public static class Live2DDotNetGui
{
    public static string RunDir { get; private set; }
    public static string InputDir { get; private set; }

    public static RunType RunType { get; private set; } = RunType.AppBuilder;

    public static bool IsCrash { get; private set; }
    public static bool IsClose { get; private set; }

    public const string Font = "resm:Live2DDotNet.Launcher.Resources.MiSans-Regular.ttf?assembly=Live2DDotNet.Launcher#MiSans";

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 |
            SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            if (e.Exception.InnerException is DBusException)
            {
                Logs.Error("Thread Error", e.Exception);
                return;
            }
            Logs.Crash("Thread Error", e.Exception);
        };

        RunType = RunType.Program;

        if (string.IsNullOrWhiteSpace(InputDir))
        {
            RunDir = AppContext.BaseDirectory;
        }
        else
        {
            RunDir = InputDir;
        }

        Console.WriteLine($"RunDir: {RunDir}");

        try
        {
            if (CheckLock())
            {
                return;
            }
            StartLock();

            Live2DDotNetCore.Init(RunDir);

            BuildAvaloniaApp()
                 .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            PathBinding.OpFile(Logs.Crash("Gui Crash", e));
            App.Close();
        }
    }

    public static void Close()
    {
        IsClose = true;
        App.Close();
    }

    public static void Reboot()
    {
        IsClose = true;
        Thread.Sleep(500);
        Process.Start($"{"Live2DDotNet.Launcher.exe"}");
        App.Close();
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        if (RunType == RunType.AppBuilder)
        {
            RunDir = AppContext.BaseDirectory;
        }

        GuiConfigUtils.Init(RunDir);

        var opt = new Win32PlatformOptions();
        var opt1 = new X11PlatformOptions();
        var opt2 = new MacOSPlatformOptions()
        {
            DisableDefaultApplicationMenuItems = true,
        };

        return AppBuilder.Configure<App>()
            .With(new FontManagerOptions
            {
                DefaultFamilyName = Font,
            })
            .With(opt)
            .With(opt1)
            .With(opt2)
            .LogToTrace()
            .UsePlatformDetect();
    }

    private static bool CheckLock()
    {
        var name = RunDir + "lock";
        if (File.Exists(name))
        {
            try
            {
                using var temp = File.Open(name, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                using var temp = File.Open(name, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                using var writer = new StreamWriter(temp);
                writer.Write(true);
                writer.Flush();
                Environment.Exit(0);
                return true;
            }
        }

        return false;
    }

    private static void StartLock()
    {
        new Thread(() =>
        {
            while (!IsClose)
            {
                TestLock();
                if (IsClose)
                {
                    return;
                }
                App.Show();
            }
        })
        {
            Name = "Live2DDotNet_Lock"
        }.Start();
    }

    private static void TestLock()
    {
        string name = RunDir + "lock";
        using var temp = File.Open(name, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        using var file = MemoryMappedFile.CreateFromFile(temp, null, 100,
            MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, false);
        using var reader = file.CreateViewAccessor();
        reader.Write(0, false);
        while (!IsClose)
        {
            Thread.Sleep(100);
            var data = reader.ReadBoolean(0);
            if (data)
                break;
        }
    }
}