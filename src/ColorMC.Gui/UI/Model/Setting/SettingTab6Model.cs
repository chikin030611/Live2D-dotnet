using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using AvaloniaEdit.Utils;
using ColorMC.Core.Objs;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Model.Items;
using ColorMC.Gui.UIBinding;
using ColorMC.Gui.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;

namespace ColorMC.Gui.UI.Model.Setting;

public partial class SettingModel
{
    private readonly List<string> _uuids = [];

    public ObservableCollection<string> GameList { get; init; } = [];

    public ObservableCollection<LockLoginModel> Locks { get; init; } = [];

    public string[] LoginList { get; init; } = UserBinding.GetLockLoginType();

    [ObservableProperty]
    private LockLoginModel? _lockSelect;

    [ObservableProperty]
    private Color _motdFontColor;
    [ObservableProperty]
    private Color _motdBackColor;

    [ObservableProperty]
    private string _serverIP;
    [ObservableProperty]
    private string? _music;

    [ObservableProperty]
    private ushort? _serverPort;

    [ObservableProperty]
    private bool _enableMotd;
    [ObservableProperty]
    private bool _enableJoin;
    [ObservableProperty]
    private bool _enableIP;
    [ObservableProperty]
    private bool _enableOneGame;
    [ObservableProperty]
    private bool _enableOneLogin;
    [ObservableProperty]
    private bool _enableMusic;
    [ObservableProperty]
    private bool _slowVolume;
    [ObservableProperty]
    private bool _runPause;
    [ObservableProperty]
    private bool _enableUI;
    [ObservableProperty]
    private bool _loop;

    [ObservableProperty]
    private int _game = -1;
    [ObservableProperty]
    private int _volume;

    private bool _serverLoad = true;

    partial void OnLoopChanged(bool value)
    {
        if (_serverLoad)
            return;
    }

    partial void OnEnableUIChanged(bool value)
    {
        if (_serverLoad)
            return;

        ConfigBinding.SetUI(value);
    }

    partial void OnEnableOneLoginChanged(bool value)
    {
        SetLoginLock();
    }

    partial void OnMotdFontColorChanged(Color value)
    {
        SetIP();
    }

    partial void OnMotdBackColorChanged(Color value)
    {
        SetIP();
    }

    partial void OnEnableOneGameChanged(bool value)
    {
        SetOneGame();
    }

    partial void OnGameChanged(int value)
    {
        SetOneGame();
    }

    partial void OnServerPortChanged(ushort? value)
    {
        SetIP();
    }

    partial void OnIPChanged(string value)
    {
        SetIP();
    }

    partial void OnEnableMotdChanged(bool value)
    {
        EnableIP = EnableJoin || EnableMotd;

        SetIP();
    }

    partial void OnEnableJoinChanged(bool value)
    {
        EnableIP = EnableJoin || EnableMotd;

        SetIP();
    }

    [RelayCommand]
    public async Task AddLockLogin()
    {
        var model = new AddLockLoginModel();
        var res = await DialogHost.Show(model, "AddLockLogin");
        if (res is true)
        {
            if (model.Index == 0)
            {
                foreach (var item in Locks)
                {
                    if (item.AuthType == AuthType.OAuth)
                    {
                        Model.Show(App.Lang("SettingWindow.Tab6.Error4"));
                        return;
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.InputText)
                    || string.IsNullOrWhiteSpace(model.InputText1))
                {
                    Model.Show(App.Lang("SettingWindow.Tab6.Error5"));
                    return;
                }
                foreach (var item in Locks)
                {
                    if (item.Name == model.InputText)
                    {
                        Model.Show(App.Lang("SettingWindow.Tab6.Error6"));
                        return;
                    }
                }
            }

            Locks.Add(new(this, new()
            {
                Name = model.InputText,
                Data = model.InputText1,
                Type = (AuthType)(model.Index + 1)
            }));

            SetLoginLock();
        }
    }

    public void Delete(LockLoginModel model)
    {
        Locks.Remove(model);

        SetLoginLock();
    }

    private void SetLoginLock()
    {
        if (_serverLoad)
            return;

        var list = new List<LockLoginSetting>();
        foreach (var item in Locks)
        {
            list.Add(item.login);
        }

        ConfigBinding.SetLoginLock(EnableOneLogin, list);
    }

    private void SetIP()
    {
        if (_serverLoad)
            return;

        ConfigBinding.SetMotd(ServerIP, ServerPort ?? 0, EnableMotd,
            EnableJoin, MotdFontColor.ToString(), MotdBackColor.ToString());
    }

    private void SetOneGame()
    {
        if (_serverLoad)
            return;

        ConfigBinding.SetLockGame(EnableOneGame, Game == -1 ? null : _uuids[Game]);
    }
}
