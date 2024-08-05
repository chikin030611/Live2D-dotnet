using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using ColorMC.Core.Config;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Manager;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Model.Items;
using ColorMC.Gui.UIBinding;
using ColorMC.Gui.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Live2DCSharpSDK.Framework.Core;

namespace ColorMC.Gui.UI.Model.Setting;

public partial class SettingModel
{
    [ObservableProperty]
    private Color _mainColor;

    [ObservableProperty]
    private bool _windowMode;
    [ObservableProperty]
    private bool _isDefaultFont;
    [ObservableProperty]
    private bool _enableFontList;
    [ObservableProperty]
    private bool _enablePicResize;
    [ObservableProperty]
    private bool _isAutoColor;
    [ObservableProperty]
    private bool _isLightColor;
    [ObservableProperty]
    private bool _isDarkColor;
    [ObservableProperty]
    private bool _enableRGB;
    [ObservableProperty]
    private bool _enableWindowTran;
    [ObservableProperty]
    private bool _enableWindowMode = true;
    [ObservableProperty]
    private bool _coreInstall;
    [ObservableProperty]
    private bool _amFade;
    [ObservableProperty]
    private bool _enableBG;
    [ObservableProperty]
    private bool _enableLive2D;
    [ObservableProperty]
    private bool _lowFps;

    [ObservableProperty]
    private bool _isHead1;
    [ObservableProperty]
    private bool _isHead2;
    [ObservableProperty]
    private bool _isHead3;
    [ObservableProperty]
    private int _headX;
    [ObservableProperty]
    private int _headY;

    [ObservableProperty]
    private LanguageType _language;
    [ObservableProperty]
    private int _picEffect;
    [ObservableProperty]
    private int _picTran;
    [ObservableProperty]
    private int _picResize;
    [ObservableProperty]
    private int _windowTranType;
    [ObservableProperty]
    private int _rgbV1;
    [ObservableProperty]
    private int _rgbV2;
    [ObservableProperty]
    private int _l2dWidth;
    [ObservableProperty]
    private int _l2dHeight;
    [ObservableProperty]
    private int _amTime;
    [ObservableProperty]
    private int _l2dPos;

    [ObservableProperty]
    private string? _pic;
    [ObservableProperty]
    private string? _live2DModel;

    [ObservableProperty]
    private string _live2DCoreState;

    private bool _load = true;

    partial void OnLowFpsChanged(bool value)
    {
        if (_load)
            return;

        ConfigBinding.SetLive2DMode(value);
    }

    partial void OnL2dPosChanged(int value)
    {
        if (_load)
            return;

        ConfigBinding.SetLive2DSize(L2dWidth, L2dHeight, L2dPos);
    }

    partial void OnEnableLive2DChanged(bool value)
    {
        if (_load)
            return;

        ConfigBinding.SetLive2D(value);
    }

    partial void OnL2dWidthChanged(int value)
    {
        if (_load)
            return;

        ConfigBinding.SetLive2DSize(L2dWidth, L2dHeight, L2dPos);
    }

    partial void OnL2dHeightChanged(int value)
    {
        if (_load)
            return;

        ConfigBinding.SetLive2DSize(L2dWidth, L2dHeight, L2dPos);
    }

    async partial void OnEnablePicResizeChanged(bool value)
    {
        if (_load)
            return;

        if (value)
        {
            Model.Progress(App.Lang("SettingWindow.Tab2.Info2"));
            await ConfigBinding.SetBackLimit(value, PicResize);
            Model.ProgressClose();
        }
    }

    [RelayCommand] 
    public async Task InstallCore()
    {
        var file = await PathBinding.SelectFile(FileType.Live2DCore);
        if (file.Item1 != null)
        {
            Model.Progress(App.Lang("SettingWindow.Tab2.Info11"));
            var res = await BaseBinding.SetLive2DCore(file.Item1);
            Model.ProgressClose();
            if (!res)
            {
                Model.Show(App.Lang("SettingWindow.Tab2.Error4"));
            }
            else
            {
                ColorMCGui.Reboot();
            }
        }
    }

    [RelayCommand]
    public void DeleteLive2D()
    {
        Live2DModel = "";

        ConfigBinding.DeleteLive2D();
    }

    [RelayCommand]
    public async Task OpenLive2D()
    {
        var file = await PathBinding.SelectFile(FileType.Live2D);
        if (file.Item1 != null)
        {
            Live2DModel = file.Item1;

            if (_load)
                return;

            SetLive2D();
        }
    }

    [RelayCommand]
    public void SetLive2D()
    {
        if (_load)
            return;

        if (string.IsNullOrWhiteSpace(Live2DModel))
        {
            Model.Show(App.Lang("SettingWindow.Tab2.Error3"));
            return;
        }
        Model.Progress(App.Lang("SettingWindow.Tab2.Info2"));
        ConfigBinding.SetLive2D(Live2DModel);
        Model.ProgressClose();

        Model.Notify(App.Lang("SettingWindow.Tab2.Info12"));
    }

    public void LoadUISetting()
    {
        _load = true;

        var config = GuiConfigUtils.Config;
        if (config is { } con)
        {
            Live2DModel = con.Live2D.Model;
            L2dHeight = con.Live2D.Height;
            L2dWidth = con.Live2D.Width;
            EnableLive2D = con.Live2D.Enable;
            L2dPos = con.Live2D.Pos;
            LowFps = con.Live2D.LowFps;
        }
        var config1 = ConfigUtils.Config;
        if (config1 is { } con1)
        {
            Language = con1.Language;
        }

        try
        {
            var version = CubismCore.Version();

            uint major = (version & 0xFF000000) >> 24;
            uint minor = (version & 0x00FF0000) >> 16;
            uint patch = version & 0x0000FFFF;
            uint vesionNumber = version;

            Live2DCoreState = $"Version: {major:0}.{minor:0}.{patch:0000} ({vesionNumber})";
            CoreInstall = true;
        }
        catch
        {
            Live2DCoreState = App.Lang("SettingWindow.Tab2.Error2");
            CoreInstall = false;
        }

        _load = false;
    }
}
