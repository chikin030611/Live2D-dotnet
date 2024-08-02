using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ColorMC.Gui.UI.Model.Items;
using ColorMC.Gui.UIBinding;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ColorMC.Gui.UI.Model.Main;

public partial class MainModel
{
    public ObservableCollection<NewsItemModel> News { get; init; } = [];

    [ObservableProperty]
    private string? _displayNews;

    [ObservableProperty]
    private bool _isLoadNews;
    [ObservableProperty]
    private bool _isHaveNews;

    [ObservableProperty]
    private Bitmap? _newsImage;
}
