using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using ColorMC.Gui.Objs;
using ColorMC.Gui.Utils;
using ColorMC.Gui.UIBinding;
using Tmds.DBus.Protocol;

namespace ColorMC.Gui;

public static class ColorMCGui
{
    public static string RunDir { get; private set; }
    public static string InputDir { get; private set; }

    public static RunType RunType { get; private set; } = RunType.AppBuilder;

    public static bool IsCrash { get; private set; }
    public static bool IsClose { get; private set; }

    public const string Font = "resm:ColorMC.Launcher.Resources.MiSans-Regular.ttf?assembly=ColorMC.Launcher#MiSans";

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 |
            SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        TaskScheduler.UnobservedTaskException += (object? sender, UnobservedTaskExceptionEventArgs e) =>
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

            ColorMCCore.Init(RunDir);

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
        Process.Start($"{("ColorMC.Launcher.exe")}");
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
            Name = "ColorMC_Lock"
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