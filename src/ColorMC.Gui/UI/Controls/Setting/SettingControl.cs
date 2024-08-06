using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using ColorMC.Gui.Manager;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Model;
using ColorMC.Gui.UI.Model.Setting;

namespace ColorMC.Gui.UI.Controls.Setting;

public partial class SettingControl : MenuControl
{
    private Tab2Control _tab2;

    private readonly int _needJava;

    public SettingControl()
    {
        Title = App.Lang("SettingWindow.Title");
        UseName = ToString() ?? "SettingControl";
    }

    public SettingControl(int mainversion) : this()
    {
        _needJava = mainversion;
    }

    public override Task<bool> OnKeyDown(object? sender, KeyEventArgs e)
    {
        return Task.FromResult(false);
    }

    public override void IPointerPressed(PointerPressedEventArgs e)
    {
    }

    public override void Closed()
    {
        WindowManager.SettingWindow = null;
    }

    public void GoTo(SettingType type)
    {
        var model = (DataContext as SettingModel)!;
        switch (type)
        {
            case SettingType.Normal:
                model.NowView = 0;
                break;
            case SettingType.SetJava:
                model.NowView = 3;
                break;
            case SettingType.Net:
                model.NowView = 1;
                break;
        }
    }

    public override void Opened()
    {
        Window.SetTitle(Title);

        (DataContext as SettingModel)!.LoadUISetting();
    }

    public override void SetModel(BaseModel model)
    {
        DataContext = new SettingModel(model);
    }

    protected override Control ViewChange(int old, int index)
    {
        var model = (DataContext as SettingModel)!;
        switch (old)
        {
            case 6:
                model.RemoveChoise();
                break;
        }
        switch (index)
        {
            case 0:
                model.LoadUISetting();
                _tab2 ??= new();
                return _tab2;
            default:
                throw new InvalidEnumArgumentException();
        }
    }

    public override Bitmap GetIcon()
    {
        return ImageManager.GameIcon;
    }
}
