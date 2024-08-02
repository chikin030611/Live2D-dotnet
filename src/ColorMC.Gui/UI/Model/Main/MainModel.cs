using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

namespace ColorMC.Gui.UI.Model.Main;

public partial class MainModel : TopModel
{
    public const string SwitchView = "SwitchView";

    public bool IsLaunch;
    public bool IsFirst = true;

    public bool IsPhone { get; } = SystemInfo.Os == OsType.Android;

    private readonly Semaphore _semaphore = new(0, 2);

    private bool _isplay = true;
    private bool _isCancel;

    [ObservableProperty]
    private (string, ushort) _server;

    [ObservableProperty]
    private bool _motdDisplay;
    [ObservableProperty]
    private bool _isGameError;

    [ObservableProperty]
    private bool _sideDisplay = true;
    [ObservableProperty]
    private bool _musicDisplay;
    [ObservableProperty]
    private bool _newsDisplay;
    [ObservableProperty]
    private bool _backDisplay;

    [ObservableProperty]
    private bool _render = true;

    [ObservableProperty]
    private bool _haveUpdate;

    [ObservableProperty]
    private string _helloText;

    [ObservableProperty]
    private float _musicVolume;

    private bool _isNewUpdate;
    private string _updateStr;

    private bool _isGetNewInfo;

    public MainModel(BaseModel model) : base(model)
    {
        UserBinding.UserEdit += LoadUser;

        ShowHello();
    }


    [RelayCommand]
    public void ShowUser()
    {
        WindowManager.ShowUser();
    }

    [RelayCommand]
    public void ShowSetting()
    {
        WindowManager.ShowSetting(SettingType.Normal);
    }

    [RelayCommand]
    public void OpenNews()
    {
        NewsDisplay = true;
        SideDisplay = false;
        MotdDisplay = false;
        HelloText = App.Lang("MainWindow.Text20");
        Model.PushBack(NewBack);
        OnPropertyChanged(SwitchView);
    }

    private void ShowHello()
    {
        HelloText = App.Lang("Hello.Text1");
    }

    private void NewBack()
    {
        NewsDisplay = false;
        if (!MinMode)
        {
            SideDisplay = true;
        }
        LoadMotd();
        ShowHello();
        Model.PopBack();
        OnPropertyChanged(SwitchView);
    }

    public void LoadMotd()
    {
        var config = GuiConfigUtils.Config.ServerCustom;
        if (config != null && config?.Motd == true &&
            !string.IsNullOrWhiteSpace(config?.IP))
        {
            MotdDisplay = true;

            Server = (config.IP, config.Port);
        }
        else
        {
            MotdDisplay = false;
        }
    }

    public async void LoadDone()
    {
        LoadUser();

        LoadMotd();

        var config = GuiConfigUtils.Config;
        var config1 = ConfigUtils.Config;
        if (config.Live2D?.LowFps == true)
        {
            LowFps = true;
        }
        if (config1.Http?.CheckUpdate == true)
        {
            var data = await UpdateChecker.Check();
            if (!data.Item1)
            {
                return;
            }
            HaveUpdate = true;
            _isNewUpdate = data.Item2 || ColorMCGui.IsAot || ColorMCGui.IsMin;
            _updateStr = data.Item3!;
        }
    }

    public override void Close()
    {
    }
}
