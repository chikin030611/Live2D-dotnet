using Avalonia.Controls;
using Avalonia.Interactivity;
using ColorMC.Core.Net;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.UIBinding;
using ColorMC.Gui.Utils.LaunchSetting;

namespace ColorMC.Gui.UI.Controls.Setting;

public partial class Tab3Control : UserControl
{

    private SettingWindow Window;
    public Tab3Control()
    {
        InitializeComponent();

        ComboBox1.Items = OtherBinding.GetDownloadSources();
        Button_Set.Click += Button_Set_Click;
    }

    private void Button_Set_Click(object? sender, RoutedEventArgs e)
    {
        if (UIUtils.CheckNumb(TextBox2.Text) || UIUtils.CheckNumb(Input1.Text))
        {
            Window.Info.Show(Localizer.Instance["SettingWindow.Tab3.Error1"]);
            return;
        }
        ConfigBinding.SetHttpConfig(new()
        {
            Source = (SourceLocal)ComboBox1.SelectedIndex,
            DownloadThread = (int)Input1.Value,
            ProxyIP = TextBox1.Text,
            ProxyPort = ushort.Parse(TextBox2.Text),
            ProxyUser = TextBox3.Text,
            ProxyPassword = TextBox4.Text,
            LoginProxy = CheckBox1.IsChecked == true,
            DownloadProxy = CheckBox2.IsChecked == true,
            GameProxy = CheckBox3.IsChecked == true
        });
        Window.Info2.Show(Localizer.Instance["SettingWindow.Tab3.Info1"]);
    }

    public void SetWindow(SettingWindow window)
    {
        Window = window;
    }

    public void Load()
    {
        var config = ConfigBinding.GetAllConfig();
        if (config.Item1 != null)
        {
            ComboBox1.SelectedIndex = (int)config.Item1.Http.Source;

            Input1.Value = config.Item1.Http.DownloadThread;

            TextBox1.Text = config.Item1.Http.ProxyIP;
            TextBox2.Text = config.Item1.Http.ProxyPort.ToString();
            TextBox3.Text = config.Item1.Http.ProxyUser;
            TextBox4.Text = config.Item1.Http.ProxyPassword;

            CheckBox1.IsChecked = config.Item1.Http.LoginProxy;
            CheckBox2.IsChecked = config.Item1.Http.DownloadProxy;
            CheckBox3.IsChecked = config.Item1.Http.GameProxy;
        }
    }
}