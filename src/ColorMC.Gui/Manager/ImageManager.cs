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
    public static SKBitmap? CapeBitmap { get; private set; }
    public static Bitmap? HeadBitmap { get; private set; }

    private static readonly Dictionary<string, Bitmap> s_gameIcon = [];

    public static event Action? PicUpdate;
    public static event Action? SkinChange;

    public static void Init()
    {
        {
            using var asset = AssetLoader.Open(new Uri("resm:ColorMC.Gui.Resource.Pic.game.png"));
            GameIcon = new Bitmap(asset);
        }
        {
            using var asset1 = AssetLoader.Open(new Uri(SystemInfo.Os == OsType.MacOS
                ? "resm:ColorMC.Gui.macicon.ico" : "resm:ColorMC.Gui.icon.ico"));
            Icon = new(asset1!);
        }
        {
            using var asset1 = AssetLoader.Open(new Uri("resm:ColorMC.Gui.Resource.Pic.load.png"));
            LoadIcon = new(asset1!);
        }
    }

    public static void SetDefaultHead()
    {
        RemoveSkin();
        HeadBitmap = null;
        OnSkinLoad();
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

    public static void RemoveSkin()
    {
        SkinBitmap?.Dispose();
        CapeBitmap?.Dispose();
        HeadBitmap?.Dispose();

        HeadBitmap = null;
        SkinBitmap = null;
        CapeBitmap = null;
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

    public static void OnSkinLoad()
    {
        SkinChange?.Invoke();
    }
}
