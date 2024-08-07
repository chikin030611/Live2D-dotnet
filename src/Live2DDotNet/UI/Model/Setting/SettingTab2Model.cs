using System.Threading.Tasks;
using Avalonia.Media;
using Live2DDotNet.Config;
using Live2DDotNet.Manager;
using Live2DDotNet.UI.Model.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Live2DCSharpSDK.Framework.Core;
using Live2DDotNet;
using Live2DDotNet.Objs;
using Live2DDotNet.UIBinding;
using Live2DDotNet.Utils;

namespace Live2DDotNet.UI.Model.Setting;

public partial class SettingModel
{
    [ObservableProperty]
    private bool _coreInstall;
    [ObservableProperty]
    private bool _enableLive2D;

    [ObservableProperty]
    private int _l2dWidth;
    [ObservableProperty]
    private int _l2dHeight;
    [ObservableProperty]
    private int _l2dPos;

    [ObservableProperty]
    private string? _live2DModel;

    [ObservableProperty]
    private string _live2DCoreState;

    private bool _load = true;

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

    [RelayCommand] 
    public async Task InstallCore()
    {
        var file = await PathBinding.SelectFile(FileType.Live2DCore);
        if (file.Item1 != null)
        {
            Model.Progress("Importing Live2DCore");
            var res = await BaseBinding.SetLive2DCore(file.Item1);
            Model.ProgressClose();
            if (!res)
            {
                Model.Show("Live2DCore import failed");
            }
            else
            {
                Live2DDotNetGui.Reboot();
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
            Model.Show("No Live2D Model");
            return;
        }
        Model.Progress("Setting up");
        ConfigBinding.SetLive2D(Live2DModel);
        Model.ProgressClose();

        Model.Notify("Applied");
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
            Live2DCoreState = "Initialization error";
            CoreInstall = false;
        }

        _load = false;
    }
}
