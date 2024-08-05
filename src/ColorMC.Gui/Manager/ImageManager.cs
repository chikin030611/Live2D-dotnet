using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ColorMC.Core.LaunchPath;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Objs;
using ColorMC.Gui.Utils;

using SkiaSharp;

namespace ColorMC.Gui.Manager;

public static class ImageManager
{
    public static Bitmap GameIcon { get; private set; }
    public static Bitmap LoadIcon { get; private set; }
    public static WindowIcon Icon { get; private set; }

    public static Bitmap? BackBitmap { get; private set; }
    public static SKBitmap? SkinBitmap { get; private set; }

    private static readonly Dictionary<string, Bitmap> s_gameIcon = [];

    public static event Action? PicUpdate;

    public static void Init()
    {
    }

    public static void RemoveImage()
    {
        var image = BackBitmap;
        BackBitmap = null;
        OnPicUpdate();
        image?.Dispose();
    }

    public static async Task LoadImage()
    {
        var config = GuiConfigUtils.Config;
        RemoveImage();
        var file = config.BackImage;
        if (string.IsNullOrWhiteSpace(file))
        {
            file = "https://api.dujin.org/bing/1920.php";
        }

        if (config.EnableBG)
        {
            BackBitmap = await ImageUtils.MakeBackImage(
                    file, config.BackEffect,
                    config.BackLimit ? config.BackLimitValue : 100);
        }

        OnPicUpdate();
        ThemeManager.Init();
        FuntionUtils.RunGC();
    }

    public static Bitmap? GetGameIcon(GameSettingObj obj)
    {
        if (s_gameIcon.TryGetValue(obj.UUID, out var image))
        {
            return image;
        }
        var file = obj.GetIconFile();
        if (File.Exists(file))
        {
            var icon = new Bitmap(file);
            s_gameIcon.Add(obj.UUID, icon);

            return icon;
        }

        return null;
    }

    public static void OnPicUpdate()
    {
        PicUpdate?.Invoke();
    }
}
