using System;
using Avalonia;
using Live2DDotNet;

#if !DEBUG
using Avalonia.Media;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Runtime.InteropServices;
#endif

namespace Live2DDotNet.Launcher;

#if !DEBUG
internal static class GuiLoad
{
    public static void Load()
    {
        Program.MainCall = Gui.Live2DDotNetGui.Main;
        Program.BuildApp = Gui.Live2DDotNetGui.BuildAvaloniaApp;
        Program.SetBaseSha1 = Gui.Live2DDotNetGui.SetBaseSha1;
        Program.SetRuntimeState = Gui.Live2DDotNetGui.SetRuntimeState;
        Program.SetInputDir = Gui.Live2DDotNetGui.SetInputDir;
    }

    public static void Run(string[] args, bool crash)
    {
        Gui.Live2DDotNetGui.SetCrash(crash);
        Gui.Live2DDotNetGui.Main(args);
    }
}
#endif

public static class Program
{
    public const string Font = "resm:Live2DDotNet.Launcher.Resources.MiSans-Regular.ttf?assembly=Live2DDotNet.Launcher#MiSans";

#if !DEBUG

#if MIN
    public const bool IsMin = true;
#else
    public const bool IsMin = false;
#endif

#if AOT
    public const bool Aot = true;
#else
    public const bool Aot = false;
#endif

    /// <summary>
    /// 加载路径
    /// </summary>
    public const string TopVersion = "A27.1";

    public static readonly string[] BaseSha1 =
    [
        "87d7ffa78a7f7fb45c0e1d5acccfad7355d91da6",
        "0bb166983f649fff298623efc91a2646b502bb9f",
        "770c0aac76cb452515a0ae64a58b322d7f39553c",
        "bcd5de56688a4824137dca3cdfd44a76f7094804"
    ];

    public delegate void IN(string[] args);
    public delegate void IN2(bool aot, bool min);
    public delegate AppBuilder IN1();
    public delegate void IN3(string dir);

    public static IN MainCall { get; set; }
    public static IN1 BuildApp { get; set; }
    public static IN SetBaseSha1 { get; set; }
    public static IN2 SetRuntimeState { get; set; }
    public static IN3 SetInputDir { get; set; }

    private static string _loadDir;
    private static string _inputDir;

    private static bool _isDll;
    private static bool _isError;
#endif

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
#if !DEBUG
        var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Live2DDotNet/run";
        if (File.Exists(path))
        {
            var dir = File.ReadAllText(path);
            if (Directory.Exists(dir))
            {
                _inputDir = dir;
            }
        }

        if (string.IsNullOrWhiteSpace(_inputDir))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _inputDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Live2DDotNet/";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _inputDir = "/Users/shared/Live2DDotNet/";
            }
            else
            {
                _inputDir = AppContext.BaseDirectory;
            }
        }

        if (!_inputDir.EndsWith('/'))
        {
            _inputDir += "/";
            _inputDir = Path.GetFullPath(_inputDir);
        }

        try
        {
            if (!Directory.Exists(_inputDir))
            {
                Directory.CreateDirectory(_inputDir);
            }
            File.Create(_inputDir + "temp").Close();
        }
        catch
        {
            //有没有权限写文件
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
            return;
        }

        _loadDir = _inputDir + "dll";

        Console.WriteLine($"LoadDir: {_loadDir}");
#endif
        try
        {
#if DEBUG
            Live2DDotNetGui.Main(args);
#else

#if !AOT
            Load();
#else
            GuiLoad.Load();
#endif
            SetInputDir(_inputDir);
            SetRuntimeState(Aot, IsMin);
            SetBaseSha1(BaseSha1);
            MainCall(args);
#endif
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
#if !DEBUG
            if (_isDll)
            {
                _isError = true;
                File.Delete($"{_loadDir}/Live2DDotNet.dll");
                File.Delete($"{_loadDir}/Live2DDotNet.pdb");
                File.Delete($"{_loadDir}/Live2DDotNet.dll");
                File.Delete($"{_loadDir}/Live2DDotNet.pdb");

                GuiLoad.Run(args, _isError);
            }
#endif
        }
    }

#if DEBUG
    public static AppBuilder BuildAvaloniaApp()
    {
        return Live2DDotNetGui.BuildAvaloniaApp();
    }
#else
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                .With(new FontManagerOptions
                {
                    DefaultFamilyName = Font,
                })
                .LogToTrace()
                .UsePlatformDetect();
    }

#if !AOT
    private static bool NotHaveDll()
    {
        return File.Exists($"{_loadDir}/Live2DDotNet.dll")
            && File.Exists($"{_loadDir}/Live2DDotNet.pdb")
            && File.Exists($"{_loadDir}/Live2DDotNet.dll")
            && File.Exists($"{_loadDir}/Live2DDotNet.pdb");
    }

    private static void Load()
    {
        if (!NotHaveDll())
        {
            GuiLoad.Load();
        }
        else
        {
            try
            {
                var context = new AssemblyLoadContext("Live2DDotNet", true);
                {
                    using var file = File.OpenRead($"{_loadDir}/Live2DDotNet.dll");
                    using var file1 = File.OpenRead($"{_loadDir}/Live2DDotNet.pdb");
                    context.LoadFromStream(file, file1);
                }
                {
                    using var file = File.OpenRead($"{_loadDir}/Live2DDotNet.dll");
                    using var file1 = File.OpenRead($"{_loadDir}/Live2DDotNet.pdb");
                    context.LoadFromStream(file, file1);
                }

                var item = context.Assemblies
                                    .Where(x => x.GetName().Name == "Live2DDotNet")
                                    .ToList()[0];

                var mis = item.GetTypes().Where(x => x.FullName == "Live2DDotNet.Live2DDotNetCore").ToList()[0];

                var temp = mis.GetField("Version");
                var version = temp?.GetValue(null) as string;
                if (version?.StartsWith(TopVersion) != true)
                {
                    context.Unload();
                    GuiLoad.Load();

                    File.Delete($"{_loadDir}/Live2DDotNet.dll");
                    File.Delete($"{_loadDir}/Live2DDotNet.pdb");
                    File.Delete($"{_loadDir}/Live2DDotNet.dll");
                    File.Delete($"{_loadDir}/Live2DDotNet.pdb");

                    return;
                }

                var item1 = context.Assemblies
                               .Where(x => x.GetName().Name == "Live2DDotNet")
                               .ToList()[0];

                var mis1 = item1.GetTypes().Where(x => x.FullName == "Live2DDotNet.Live2DDotNetGui").ToList()[0];

                MainCall = (Delegate.CreateDelegate(typeof(IN),
                        mis1.GetMethod("Main")!) as IN)!;

                BuildApp = (Delegate.CreateDelegate(typeof(IN1),
                        mis1.GetMethod("BuildAvaloniaApp")!) as IN1)!;

                SetBaseSha1 = (Delegate.CreateDelegate(typeof(IN),
                        mis1.GetMethod("SetBaseSha1")!) as IN)!;

                SetRuntimeState = (Delegate.CreateDelegate(typeof(IN2),
                       mis1.GetMethod("SetRuntimeState")!) as IN2)!;

                SetInputDir = (Delegate.CreateDelegate(typeof(IN3),
                       mis1.GetMethod("SetInputDir")!) as IN3)!;

                _isDll = true;
            }
            catch
            {
                _isError = true;
                GuiLoad.Load();
            }
        }
    }
#endif

#endif
}
