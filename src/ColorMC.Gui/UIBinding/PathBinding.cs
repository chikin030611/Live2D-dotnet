using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ColorMC.Core;
using ColorMC.Core.Downloader;
using ColorMC.Core.Game;
using ColorMC.Core.Helpers;
using ColorMC.Core.LaunchPath;
using ColorMC.Core.Net.Apis;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.CurseForge;
using ColorMC.Core.Objs.Minecraft;
using ColorMC.Core.Objs.Modrinth;
using ColorMC.Core.Utils;
using ColorMC.Gui.Manager;
using ColorMC.Gui.Utils;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using SkiaSharp;

namespace ColorMC.Gui.UIBinding;

public static class PathBinding
{
    /// <summary>
    /// 提升权限
    /// </summary>
    /// <param name="path">文件</param>
    public static void Chmod(string path)
    {
        try
        {
            using var p = new Process();
            p.StartInfo.FileName = "sh";
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.StandardInput.WriteLine("chmod a+x " + path);

            p.StandardInput.WriteLine("exit");
            p.WaitForExit();
        }
        catch (Exception e)
        {
            Logs.Error("chmod error", e);
        }
    }

    /// <summary>
    /// 在资源管理器打开路径
    /// </summary>
    /// <param name="item">路径</param>
    private static void OpPath(string item)
    {
        item = Path.GetFullPath(item);
        switch (SystemInfo.Os)
        {
            case OsType.Windows:
                Process.Start("explorer", $"{item}");
                break;
            case OsType.Linux:
                Process.Start("xdg-open", '"' + item + '"');
                break;
            case OsType.MacOS:
                Process.Start("open", '"' + item + '"');
                break;
        }
    }

    /// <summary>
    /// 打开路径
    /// </summary>
    /// <param name="obj">游戏实例</param>
    /// <param name="type">路径类型</param>
    public static void OpPath(GameSettingObj obj, PathType type)
    {
        switch (type)
        {
            case PathType.ShaderpacksPath:
                OpPath(obj.GetShaderpacksPath());
                break;
            case PathType.ResourcepackPath:
                OpPath(obj.GetResourcepacksPath());
                break;
            case PathType.WorldBackPath:
                OpPath(obj.GetWorldBackupPath());
                break;
            case PathType.SavePath:
                OpPath(obj.GetSavesPath());
                break;
            case PathType.GamePath:
                OpPath(obj.GetGamePath());
                break;
            case PathType.SchematicsPath:
                OpPath(obj.GetSchematicsPath());
                break;
            case PathType.ScreenshotsPath:
                OpPath(obj.GetScreenshotsPath());
                break;
            case PathType.ModPath:
                OpPath(obj.GetModsPath());
                break;
            case PathType.BasePath:
                OpPath(obj.GetBasePath());
                break;
        }
    }

    /// <summary>
    /// 打开路径
    /// </summary>
    /// <param name="type">路径类型</param>
    public static void OpPath(PathType type)
    {
        switch (type)
        {
            case PathType.BasePath:
                OpPath(ColorMCCore.BaseDir);
                break;
            case PathType.RunPath:
                OpPath(AppContext.BaseDirectory);
                break;
            case PathType.DownloadPath:
                OpPath(DownloadManager.DownloadDir);
                break;
            case PathType.JavaPath:
                OpPath(JvmPath.BaseDir + JvmPath.Name1);
                break;
            case PathType.PicPath:
                OpPath(ImageUtils.Local);
                break;
        }
    }

    /// <summary>
    /// 打开路径
    /// </summary>
    /// <param name="obj">世界存储</param>
    public static void OpPath(WorldObj obj)
    {
        OpPath(obj.Local);
    }

    /// <summary>
    /// 在资源管理器打开文件
    /// </summary>
    /// <param name="item">文件</param>
    public static void OpFile(string item)
    {
        switch (SystemInfo.Os)
        {
            case OsType.Windows:
                Process.Start("explorer",
                    $@"/select,{item}");
                break;
            case OsType.Linux:
                try
                {
                    Process.Start("nautilus",
                        '"' + item + '"');
                }
                catch
                {
                    Process.Start("dolphin",
                        '"' + item + '"');
                }
                break;
            case OsType.MacOS:
                var file1 = new FileInfo(item);
                Process.Start("open", '"' + file1.Directory?.FullName + '"');
                break;
        }
    }

    /// <summary>
    /// 选择路径
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="type">类型</param>
    /// <returns>路径</returns>
    public static async Task<string?> SelectPath(PathType type)
    {
        var top = App.TopLevel;
        if (top == null)
        {
            return null;
        }
        switch (type)
        {
            case PathType.ServerPackPath:
                var res = await top.StorageProvider.OpenFolderPickerAsync(new()
                {
                    Title = App.Lang("PathBinding.Text2")
                });
                if (res?.Any() == true)
                {
                    return res[0].GetPath();
                }
                break;
            case PathType.GamePath:
                res = await top.StorageProvider.OpenFolderPickerAsync(new()
                {
                    Title = App.Lang("PathBinding.Text6")
                });
                if (res?.Any() == true)
                {
                    return res[0].GetPath();
                }
                break;
            case PathType.RunDir:
                res = await top.StorageProvider.OpenFolderPickerAsync(new()
                {
                    Title = App.Lang("SettingWindow.Tab1.Info14")
                });
                if (res?.Any() == true)
                {
                    return res[0].GetPath();
                }
                break;
        }

        return null;
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="title">标题</param>
    /// <param name="ext">后缀</param>
    /// <param name="name">名字</param>
    /// <returns>文件路径</returns>
    private static Task<IStorageFile?> SaveFile(TopLevel window, string title, string ext, string name)
    {
        return window.StorageProvider.SaveFilePickerAsync(new()
        {
            Title = title,
            DefaultExtension = ext,
            SuggestedFileName = name
        });
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="type">类型</param>
    /// <param name="arg">参数</param>
    /// <returns>结果</returns>
    public static async Task<bool?> SaveFile(FileType type, object[]? arg)
    {
        var top = App.TopLevel;
        if (top == null)
        {
            return false;
        }

        return null;
    }

    /// <summary>
    /// 打开文件
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="title">标题</param>
    /// <param name="ext">后缀</param>
    /// <param name="name">名字</param>
    /// <param name="multiple">多选</param>
    /// <param name="storage">首选路径</param>
    /// <returns></returns>
    private static async Task<IReadOnlyList<IStorageFile>?> SelectFile(TopLevel? window, string title,
        string[]? ext, string name, bool multiple = false, DirectoryInfo? storage = null)
    {
        if (window == null)
            return null;

        var defaultFolder = storage == null ? null : await window.StorageProvider.TryGetFolderFromPathAsync(storage.FullName);

        return await window.StorageProvider.OpenFilePickerAsync(new()
        {
            Title = title,
            AllowMultiple = multiple,
            SuggestedStartLocation = defaultFolder,
            FileTypeFilter = ext == null ? null : new List<FilePickerFileType>()
            {
                new(name)
                {
                     Patterns = new List<string>(ext)
                }
            }
        });
    }

    private static readonly string[] EXE = ["*.exe"];
    private static readonly string[] ZIP = ["*.zip", "*.tar.xz", "*.tar.gz"];
    private static readonly string[] JSON = ["*.json"];
    private static readonly string[] MODPACK = ["*.zip", "*.mrpack"];
    private static readonly string[] PICFILE = ["*.png", "*.jpg", "*.bmp"];
    private static readonly string[] AUDIO = ["*.mp3", "*.wav"];
    private static readonly string[] MODEL = ["*.model3.json"];
    private static readonly string[] HEADFILE = ["*.png"];
    private static readonly string[] ZIPFILE = ["*.zip"];
    private static readonly string[] JARFILE = ["*.jar"];

    /// <summary>
    /// 打开文件
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="type">类型</param>
    /// <returns>路径</returns>
    public static async Task<(string?, string?)> SelectFile(FileType type)
    {
        var top = App.TopLevel;
        if (top == null)
        {
            return (null, null);
        }

        switch (type)
        {
            case FileType.Java:
                var res = await SelectFile(top,
                    App.Lang("PathBinding.Text26"),
                    SystemInfo.Os == OsType.Windows ? EXE : null,
                    App.Lang("PathBinding.Text27"),
                    storage: JavaBinding.GetSuggestedStartLocation());
                if (res?.Any() == true)
                {
                    var file = res[0].GetPath();
                    if (file == null)
                        return (null, null);
                    if (SystemInfo.Os == OsType.Windows && file.EndsWith("java.exe"))
                    {
                        var file1 = file[..^4] + "w.exe";
                        if (File.Exists(file1))
                            return (file1, "javaw.exe");
                    }

                    return (file, res[0].Name);
                }
                break;
            case FileType.JavaZip:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text28"),
                    ZIP,
                    App.Lang("PathBinding.Text29"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.Config:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text30"),
                    JSON,
                    App.Lang("PathBinding.Text31"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.AuthConfig:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text32"),
                    JSON,
                    App.Lang("PathBinding.Text33"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.ModPack:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text4"),
                    MODPACK,
                    App.Lang("PathBinding.Text5"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.Pic:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text34"),
                    PICFILE,
                    App.Lang("PathBinding.Text35"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.Music:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text11"),
                    AUDIO,
                    App.Lang("PathBinding.Text12"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.Live2D:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text36"),
                    MODEL,
                    App.Lang("PathBinding.Text37"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.Icon:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text7"),
                    PICFILE,
                    App.Lang("PathBinding.Text8"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.Head:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text9"),
                    HEADFILE,
                    App.Lang("PathBinding.Text10"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.Live2DCore:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text38"),
                    ZIPFILE,
                    App.Lang("PathBinding.Text39"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.Loader:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text24"),
                    JARFILE,
                    App.Lang("PathBinding.Text25"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.InputConfig:
                res = await SelectFile(top,
                    App.Lang("PathBinding.Text13"),
                    JSON,
                    App.Lang("PathBinding.Text14"));
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
        }

        return (null, null);
    }

    public static async Task CopyBG(string pic)
    {
        try
        {
            using var stream = ColorMCCore.PhoneReadFile(pic);
            if (stream == null)
                return;
            string file = ColorMCGui.RunDir + "BG";
            PathHelper.Delete(file);
            using var temp = File.Create(file);
            await stream.CopyToAsync(temp);
        }
        catch (Exception e)
        {
            Logs.Error(App.Lang("PathBinding.Error1"), e);
        }
    }

    public static void OpenPicFile(string screenshot)
    {
        screenshot = Path.GetFullPath(screenshot);
        switch (SystemInfo.Os)
        {
            case OsType.Windows:
                {
                    var proc = new Process();
                    proc.StartInfo.WorkingDirectory = ColorMCCore.BaseDir;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/c start \"\" \"{screenshot}\"";
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                    break;
                }
            case OsType.Linux:
                Process.Start("xdg-open",
                    '"' + screenshot + '"');
                break;
            case OsType.MacOS:
                Process.Start("open", "-a Preview " +
                    '"' + screenshot + '"');
                break;
        }
    }
}