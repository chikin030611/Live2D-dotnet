using ColorMC.Core.Config;
using ColorMC.Core.Objs;
using ColorMC.Gui.Manager;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UIBinding;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ColorMC.Gui.UI.Model.Main;

public partial class MainModel
{
    [ObservableProperty]
    private LanguageType _language;

    private bool _emptyLoad = true;


}
