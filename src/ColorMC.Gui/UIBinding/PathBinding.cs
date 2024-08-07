using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ColorMC.Gui.Objs;

namespace ColorMC.Gui.UIBinding;

public static class PathUtils
{
    /// <summary>
    /// 文件转字符串
    /// </summary>
    /// <param name="file">文件</param>
    /// <returns>路径字符串</returns>
    public static string? GetPath(this IStorageFile file)
    {
        return file.Path.LocalPath;
    }
}

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
    public static async Task<bool?> SaveFile(object[]? arg)
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

    private static readonly string[] MODEL = ["*.model3.json"];
    private static readonly string[] ZIPFILE = ["*.zip"];

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
            case FileType.Live2D:
                var res = await SelectFile(top,
                    "Select Live2D model json",
                    MODEL,
                    "Model json");
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
            case FileType.Live2DCore:
                res = await SelectFile(top,
                    "Select Live2DCore Compression",
                    ZIPFILE,
                    "Live2DCove Compression");
                if (res?.Any() == true)
                {
                    return (res[0].GetPath(), res[0].Name);
                }
                break;
        }

        return (null, null);
    }
}