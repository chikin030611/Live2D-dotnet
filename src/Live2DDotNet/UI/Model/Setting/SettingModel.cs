using Live2DDotNet.Objs;
using Live2DDotNet.Utils;
using Live2DDotNet.UI.Model.Items;
using Live2DDotNet.UIBinding;

namespace Live2DDotNet.UI.Model.Setting;

public partial class SettingModel : MenuModel
{
    public bool IsInputEnable { get; }

    private readonly string _name;

    public SettingModel(BaseModel model) : base(model)
    {
        _name = ToString() ?? "SettingModel";

        IsInputEnable = true;

        SetMenu(
        [
            new()
            {
                Icon = "/Resource/Icon/Setting/item1.svg",
                Text = "Interface Settings"
            }
        ]);
    }

    public void RemoveChoise()
    {
        Model.RemoveChoiseData(_name);
    }

    public override void Close()
    {
    }
}
