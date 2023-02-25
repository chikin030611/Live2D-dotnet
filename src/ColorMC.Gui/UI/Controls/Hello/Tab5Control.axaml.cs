using Avalonia.Controls;
using Avalonia.Interactivity;
using ColorMC.Core;
using ColorMC.Core.LaunchPath;
using ColorMC.Core.Net;
using ColorMC.Core.Net.Apis;
using ColorMC.Core.Objs;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.UIBinding;
using DynamicData;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ColorMC.Gui.UI.Controls.Hello;

public partial class Tab5Control : UserControl
{
    private readonly ObservableCollection<string> List = new();

    private bool add;

    public Tab5Control()
    {
        InitializeComponent();

        ComboBox_GameVersion.Items = List;
        ComboBox_GameVersion.SelectionChanged += GameVersion_SelectionChanged;

        ComboBox4.Items = GameBinding.GetPackType();

        Button_Next.Click += Button_Next_Click;
        Button_Add.Click += Button_Add_Click;

        Button4.Click += Button4_Click;

        CheckBox_Forge.Click += Forge_Click;
        CheckBox_Fabric.Click += Fabric_Click;
        CheckBox_Quilt.Click += Quilt_Click;
        CheckBox_Release.Click += Release_Click;
        CheckBox_Snapshot.Click += Snapshot_Click;
        CheckBox_Other.Click += Other_Click;

        ColorMCCore.PackState = PackState;
        ColorMCCore.PackUpdate = PackUpdate;
        ColorMCCore.GameOverwirte = GameOverwirte;

        Load();
    }

    private void Button4_Click(object? sender, RoutedEventArgs e)
    {
        switch (ComboBox4.SelectedIndex)
        {
            case 0:
                AddPack(PackType.ColorMC);
                break;
            case 1:
                AddPack(PackType.CurseForge);
                break;
            case 2:
                AddPack(PackType.Modrinth);
                break;
            case 3:
                AddPack(PackType.MMC);
                break;
            case 4:
                AddPack(PackType.HMCL);
                break;
        }
    }

    private async void Button_Add_Click(object? sender, RoutedEventArgs e)
    {
        var window = (VisualRoot as HelloWindow)!;
        var name = TextBox_Input1.Text;
        if (string.IsNullOrWhiteSpace(name))
        {
            window.Info.Show(App.GetLanguage("AddGameWindow.Error1"));
            return;
        }

        string? version = ComboBox_GameVersion.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(version))
        {
            window.Info.Show(App.GetLanguage("AddGameWindow.Error2"));
            return;
        }

        var loaderversion = ComboBox_LoaderVersion.SelectedItem as string;
        Loaders loader = Loaders.Normal;
        if (!string.IsNullOrWhiteSpace(loaderversion))
        {
            if (CheckBox_Forge.IsChecked == true)
            {
                loader = Loaders.Forge;
            }
            else if (CheckBox_Fabric.IsChecked == true)
            {
                loader = Loaders.Fabric;
            }
            else if (CheckBox_Quilt.IsChecked == true)
            {
                loader = Loaders.Quilt;
            }
        }

        add = true;
        var res = await GameBinding.AddGame(name, version, loader, loaderversion);
        if (!res)
        {
            window.Info.Show(App.GetLanguage("AddGameWindow.Error5"));
        }
        else
        {
            window.Info2.Show(App.GetLanguage("AddGameWindow.Info2"));
        }
    }

    private async void GameVersion_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var window = (VisualRoot as HelloWindow)!;

        CheckBox_Forge.IsEnabled = false;
        CheckBox_Fabric.IsEnabled = false;
        CheckBox_Quilt.IsEnabled = false;

        string? item = ComboBox_GameVersion.SelectedItem as string;
        if (!string.IsNullOrWhiteSpace(item))
        {
            window.Info1.Show(App.GetLanguage("AddGameWindow.Info3"));
            var list = await GameBinding.GetForgeSupportVersion();
            if (list != null && list.Contains(item))
            {
                CheckBox_Forge.IsEnabled = true;
            }

            list = await GameBinding.GetFabricSupportVersion();
            if (list != null && list.Contains(item))
            {
                CheckBox_Fabric.IsEnabled = true;
            }

            list = await GameBinding.GetQuiltSupportVersion();
            if (list != null && list.Contains(item))
            {
                CheckBox_Quilt.IsEnabled = true;
            }
            window.Info1.Close();
        }
    }

    private async void Quilt_Click(object? sender, RoutedEventArgs e)
    {
        var window = (VisualRoot as HelloWindow)!;
        if (CheckBox_Quilt.IsChecked == true)
        {
            string? item = ComboBox_GameVersion.SelectedItem as string;
            if (item == null)
                return;

            window.Info1.Show(App.GetLanguage("AddGameWindow.Info4"));
            CheckBox_Forge.IsEnabled = false;
            CheckBox_Fabric.IsEnabled = false;

            var list = await QuiltHelper.GetLoaders(item, BaseClient.Source);
            window.Info1.Close();
            if (list == null)
            {
                return;
            }

            ComboBox_LoaderVersion.IsEnabled = true;
            ComboBox_LoaderVersion.Items = list;
        }
        else
        {
            ComboBox_LoaderVersion.IsEnabled = false;
            CheckBox_Forge.IsEnabled = true;
            CheckBox_Fabric.IsEnabled = true;
            ComboBox_LoaderVersion.Items = null;
        }
    }

    private async void Fabric_Click(object? sender, RoutedEventArgs e)
    {
        var window = (VisualRoot as HelloWindow)!;
        if (CheckBox_Fabric.IsChecked == true)
        {
            string? item = ComboBox_GameVersion.SelectedItem as string;
            if (item == null)
                return;

            window.Info1.Show(App.GetLanguage("AddGameWindow.Info5"));
            CheckBox_Forge.IsEnabled = false;
            CheckBox_Quilt.IsEnabled = false;

            var list = await FabricHelper.GetLoaders(item, BaseClient.Source);
            window.Info1.Close();
            if (list == null)
            {
                return;
            }

            ComboBox_LoaderVersion.IsEnabled = true;
            ComboBox_LoaderVersion.Items = list;
        }
        else
        {
            ComboBox_LoaderVersion.IsEnabled = false;
            CheckBox_Forge.IsEnabled = true;
            CheckBox_Quilt.IsEnabled = true;
            ComboBox_LoaderVersion.Items = null;
        }
    }

    private async void Forge_Click(object? sender, RoutedEventArgs e)
    {
        var window = (VisualRoot as HelloWindow)!;
        if (CheckBox_Forge.IsChecked == true)
        {
            string? item = ComboBox_GameVersion.SelectedItem as string;
            if (item == null)
                return;

            window.Info1.Show(App.GetLanguage("AddGameWindow.Info6"));
            CheckBox_Fabric.IsEnabled = false;
            CheckBox_Quilt.IsEnabled = false;

            var list = await ForgeHelper.GetVersionList(item, BaseClient.Source);
            window.Info1.Close();
            if (list == null)
            {
                return;
            }

            ComboBox_LoaderVersion.IsEnabled = true;
            ComboBox_LoaderVersion.Items = list;
        }
        else
        {
            ComboBox_LoaderVersion.IsEnabled = false;
            CheckBox_Fabric.IsEnabled = true;
            CheckBox_Quilt.IsEnabled = true;
            ComboBox_LoaderVersion.Items = null;
        }
    }

    private async Task<bool> GameOverwirte(GameSettingObj obj)
    {
        var window = (VisualRoot as HelloWindow)!;
        window.Info1.Close();
        var test = await window.Info.ShowWait(
            string.Format(App.GetLanguage("AddGameWindow.Info7"), obj.Name));
        if (!add)
        {
            window.Info1.Show();
        }
        return test;
    }

    private void PackUpdate(int size, int now)
    {
        var window = (VisualRoot as HelloWindow)!;
        window.Info1.Progress((double)now / size);
    }

    private void PackState(CoreRunState state)
    {
        var window = (VisualRoot as HelloWindow)!;
        if (state == CoreRunState.Read)
        {
            window.Info1.Show(App.GetLanguage("AddGameWindow.Info8"));
        }
        else if (state == CoreRunState.Init)
        {
            window.Info1.NextText(App.GetLanguage("AddGameWindow.Info9"));
        }
        else if (state == CoreRunState.GetInfo)
        {
            window.Info1.NextText(App.GetLanguage("AddGameWindow.Info10"));
        }
        else if (state == CoreRunState.Download)
        {
            window.Info1.NextText(App.GetLanguage("AddGameWindow.Info11"));
            window.Info1.Progress(-1);
        }
        else if (state == CoreRunState.End)
        {
            window.Info1.Close();
        }
    }

    private void Button_Add4_Click(object? sender, RoutedEventArgs e)
    {
        AddPack(PackType.HMCL);
    }

    private void Button_Add3_Click(object? sender, RoutedEventArgs e)
    {
        AddPack(PackType.MMC);
    }

    private void Button_Add2_Click(object? sender, RoutedEventArgs e)
    {
        AddPack(PackType.CurseForge);
    }

    private void Button_Add1_Click(object? sender, RoutedEventArgs e)
    {
        AddPack(PackType.ColorMC);
    }

    private async void AddPack(PackType type)
    {
        var window = (VisualRoot as HelloWindow)!;
        add = false;
        var name = await SelectPack();
        if (name == null)
            return;

        window.Info1.Show(App.GetLanguage("AddGameWindow.Info14"));
        var res = await GameBinding.AddPack(name, type, TextBox_Input1.Text, null);
        window.Info1.Close();
        if (res.Item1)
        {
            window.Info2.Show(App.GetLanguage("AddGameWindow.Info12"));
        }
        else
        {
            window.Info.Show(App.GetLanguage("AddGameWindow.Error3"));
        }
    }

    private async Task<string?> SelectPack()
    {
        var window = (VisualRoot as HelloWindow)!;
        var file = await BaseBinding.OpFile(window,
            App.GetLanguage("AddGameWindow.Info13"),
            new string[] { "*.zip", "*.mrpack" },
            App.GetLanguage("AddGameWindow.Info15"));
        if (file.Any())
        {
            return file[0].GetPath();
        }

        return null;
    }

    private void Other_Click(object? sender, RoutedEventArgs e)
    {
        Load();
    }

    private void Snapshot_Click(object? sender, RoutedEventArgs e)
    {
        Load();
    }

    private void Release_Click(object? sender, RoutedEventArgs e)
    {
        Load();
    }

    public void Load()
    {
        List.Clear();
        List.AddRange(GameBinding.GetGameVersion(CheckBox_Release.IsChecked,
            CheckBox_Snapshot.IsChecked, CheckBox_Other.IsChecked));
    }

    private void Button_Next_Click(object? sender, RoutedEventArgs e)
    {
        var window = (VisualRoot as HelloWindow)!;
        window.Next();
    }
}
