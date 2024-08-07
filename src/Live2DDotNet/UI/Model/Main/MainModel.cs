using System.Threading;
using Live2DDotNet.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Live2DDotNet.Manager;
using Live2DDotNet.Objs;

namespace Live2DDotNet.UI.Model.Main;

public partial class MainModel : TopModel
{
    public const string SwitchView = "SwitchView";

    public bool IsLaunch;
    public bool IsFirst = true;

    private readonly Semaphore _semaphore = new(0, 2);

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

    public MainModel(BaseModel model) : base(model)
    {
    }

    [RelayCommand]
    public void ShowSetting()
    {
        WindowManager.ShowSetting(SettingType.Normal);
    }


    public override void Close()
    {
    }
}
