using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.UI.Model.Items;
using ColorMC.Gui.UIBinding;

namespace ColorMC.Gui.UI.Model.Setting;

public partial class SettingModel : MenuModel
{
    public bool Phone { get; } = false;
    public bool IsInputEnable { get; }

    private readonly string _name;

    public SettingModel(BaseModel model) : base(model)
    {
        _name = ToString() ?? "SettingModel";

        if (SystemInfo.Os == OsType.Android)
        {
            Phone = true;
            _enableWindowMode = false;
        }
        else if (SystemInfo.Os is OsType.Windows)
        {
            IsInputEnable = true;
        }

        if (!BaseBinding.SdlInit)
        {
            InputInit = false;
        }
        else
        {
            InputInit = true;
            StartRead();
            ReloadInput();
        }

        SetMenu(
        [
            new()
            {
                Icon = "/Resource/Icon/Setting/item1.svg",
                Text = App.Lang("SettingWindow.Tabs.Text2")
            }
        ]);
    }

    public void RemoveChoise()
    {
        Model.RemoveChoiseData(_name);
    }

    public override void Close()
    {
        FontList.Clear();
        InputClose();
        StopRead();
    }
}
