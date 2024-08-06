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
using ColorMC.Core.Helpers;
using ColorMC.Core.Objs;
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
    /// 在资源管理器打开文件
    /// </summary>
    /// <param name="item">文件</param>
    public static void OpFile(string item)
    {
        Process.Start("explorer", $@"/select,{item}");
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
            case FileType.Config:
                var res = await SelectFile(top,
                    App.Lang("PathBinding.Text30"),
                    JSON,
                    App.Lang("PathBinding.Text31"));
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
}