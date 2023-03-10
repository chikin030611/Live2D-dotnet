using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ColorMC.Core.Objs.ServerPack;
using ColorMC.Gui.UIBinding;

namespace ColorMC.Gui.UI.Controls.Server;

public partial class Tab1Control : UserControl
{
    private bool load = false;
    private ServerPackObj Obj1;

    public Tab1Control()
    {
        InitializeComponent();

        TextBox1.PropertyChanged += TextBox1_PropertyChanged;
        TextBox2.PropertyChanged += TextBox1_PropertyChanged;
        TextBox3.PropertyChanged += TextBox1_PropertyChanged;
        TextBox4.PropertyChanged += TextBox1_PropertyChanged;

        TextBox1.LostFocus += TextBox1_LostFocus;

        Button1.Click += Button1_Click;
        Button2.Click += Button2_Click;
    }

    private void TextBox1_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TextBox1.Text))
            return;

        if (TextBox1.Text.EndsWith("/"))
            return;

        TextBox1.Text += "/";
    }

    private void Button2_Click(object? sender, RoutedEventArgs e)
    {
        var window = App.FindRoot(VisualRoot);
        if (string.IsNullOrWhiteSpace(Obj1.Url))
        {
            window.Info.Show("基础网址为空");
            return;
        }

        if (string.IsNullOrWhiteSpace(Obj1.Version))
        {
            window.Info.Show("版本号为空");
            return;
        }
    }

    private async void Button1_Click(object? sender, RoutedEventArgs e)
    {
        var window = App.FindRoot(VisualRoot);
        var file = await BaseBinding.OpFile(window, Core.Objs.FileType.UI);
        if (file == null)
            return;

        TextBox4.Text = file;
    }

    private void TextBox1_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (load)
            return;

        if (e.Property.Name == "Text")
        {
            Save();
        }
    }

    public void Load()
    {
        if (Obj1 == null)
            return;

        load = true;
        TextBox1.Text = Obj1.Url;
        TextBox2.Text = Obj1.Version;
        TextBox3.Text = Obj1.Text;
        TextBox4.Text = Obj1.UI;
        CheckBox1.IsChecked = Obj1.LockMod;
        CheckBox2.IsChecked = Obj1.ForceUpdate;
        load = false;
    }

    public void SetObj(ServerPackObj obj1)
    {
        Obj1 = obj1;
    }

    private void Save()
    {
        Obj1.Url = TextBox1.Text;
        Obj1.Version = TextBox2.Text;
        Obj1.Text = TextBox3.Text;
        Obj1.UI = TextBox4.Text;
        Obj1.LockMod = CheckBox1.IsChecked == true;
        Obj1.ForceUpdate = CheckBox2.IsChecked == true;

        var window = App.FindRoot(VisualRoot);
        (window.Con as ServerPackControl)?.Save();
    }
}