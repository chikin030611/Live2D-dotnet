using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ColorMC.Core;
using ColorMC.Core.Utils;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Controls.User;
using ColorMC.Gui.UIBinding;
using ColorMC.Gui.Utils.LaunchSetting;
using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace ColorMC.Gui.UI.Windows;

public partial class UserWindow : Window
{
    private readonly ObservableCollection<UserDisplayObj> List = new();

    private readonly CrossFade CrossFade = new(TimeSpan.FromMilliseconds(300));
    private readonly CrossFade CrossFade1 = new(TimeSpan.FromMilliseconds(100));
    public UserWindow()
    {
        InitializeComponent();

        this.MakeItNoChrome();
        Rectangle1.MakeResizeDrag(this);
        FontFamily = Program.FontFamily;

        if (App.BackBitmap != null)
        {
            Image_Back.Source = App.BackBitmap;
        }

        DataGrid_User.Items = List;
        DataGrid_User.DoubleTapped += DataGrid_User_DoubleTapped;
        DataGrid_User.CellPointerPressed += DataGrid_User_PointerPressed;

        Button_A1.PointerLeave += Button_A1_PointerLeave;
        Button_A.PointerEnter += Button_A_PointerEnter;

        Button_D1.PointerLeave += Button_D1_PointerLeave;
        Button_D.PointerEnter += Button_D_PointerEnter;

        Button_S1.PointerLeave += Button_S1_PointerLeave;
        Button_S.PointerEnter += Button_S_PointerEnter;

        Button_A1.Click += Button_A1_Click;
        Button_D1.Click += Button_D1_Click;
        Button_S1.Click += Button_S1_Click;

        Button_Cancel.Click += Button_Cancel_Click;
        Button_Add.Click += Button_Add_Click;

        Expander_A.ContentTransition = CrossFade1;
        Expander_D.ContentTransition = CrossFade1;
        Expander_S.ContentTransition = CrossFade1;

        Closed += UserWindow_Closed;
        Opened += UserWindow_Opened;

        ComboBox_UserType.SelectionChanged += UserType_SelectionChanged;
        ComboBox_UserType.Items = UserBinding.GetUserTypes();

        Update();

        Load();
    }

    private void DataGrid_User_PointerPressed(object? sender,
        DataGridCellPointerPressedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var user = DataGrid_User.SelectedItem as UserDisplayObj;
            if (user == null)
                return;

            if (e.PointerPressedEventArgs.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                new UserFlyout(this, user).ShowAt(this, true);
            }
        });
    }

    public void Update()
    {
        App.Update(this, Image_Back, Grid1);
    }

    private void DataGrid_User_DoubleTapped(object? sender, RoutedEventArgs e)
    {
        var item = DataGrid_User.SelectedItem as UserDisplayObj;
        if (item == null)
        {
            Info.Show(Localizer.Instance["UserWindow.Error1"]);
            return;
        }

        UserBinding.SetLastUser(item.UUID, item.AuthType);

        Info2.Show(Localizer.Instance["UserWindow.Ok1"]);
        Load();
    }

    private async void Button_Add_Click(object? sender, RoutedEventArgs e)
    {
        Button_Add.IsEnabled = false;
        switch (ComboBox_UserType.SelectedIndex)
        {
            case 0:
                string name = TextBox_Input1.Text;
                if (string.IsNullOrWhiteSpace(name))
                {
                    Info.Show(Localizer.Instance["UserWindow.Error2"]);
                    break;
                }
                var res = await UserBinding.AddUser(0, name, null);
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                }
                Info2.Show(Localizer.Instance["UserWindow.Ok2"]);
                TextBox_Input1.Text = "";
                break;
            case 1:
                CoreMain.LoginOAuthCode = LoginOAuthCode;
                Info1.Show(Localizer.Instance["UserWindow.Info1"]);
                res = await UserBinding.AddUser(1, null);
                Info3.Close();
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                }
                Info2.Show(Localizer.Instance["UserWindow.Ok2"]);
                TextBox_Input1.Text = "";
                break;
            case 2:
                var server = TextBox_Input1.Text;
                if (server.Length != 32)
                {
                    Info.Show(Localizer.Instance["UserWindow.Error3"]);
                    break;
                }
                if (string.IsNullOrWhiteSpace(TextBox_Input2.Text) ||
                    string.IsNullOrWhiteSpace(TextBox_Input3.Text))
                {
                    Info.Show(Localizer.Instance["UserWindow.Error2"]);
                    break;
                }
                Info1.Show(Localizer.Instance["UserWindow.Info2"]);
                res = await UserBinding.AddUser(2, server,
                    TextBox_Input2.Text, TextBox_Input3.Text);
                Info1.Close();
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                    break;
                }
                Info2.Show(Localizer.Instance["UserWindow.Ok2"]);
                TextBox_Input1.Text = "";
                break;
            case 3:
                server = TextBox_Input1.Text;
                if (string.IsNullOrWhiteSpace(server))
                {
                    Info.Show(Localizer.Instance["UserWindow.Error4"]);
                    break;
                }
                if (string.IsNullOrWhiteSpace(TextBox_Input2.Text) ||
                   string.IsNullOrWhiteSpace(TextBox_Input3.Text))
                {
                    Info.Show(Localizer.Instance["UserWindow.Error2"]);
                    break;
                }
                Info1.Show(Localizer.Instance["UserWindow.Info2"]);
                res = await UserBinding.AddUser(3, server,
                    TextBox_Input2.Text, TextBox_Input3.Text);
                Info1.Close();
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                    break;
                }
                Info2.Show(Localizer.Instance["UserWindow.Ok2"]);
                TextBox_Input1.Text = "";
                break;
            case 4:
                if (string.IsNullOrWhiteSpace(TextBox_Input2.Text) ||
                   string.IsNullOrWhiteSpace(TextBox_Input3.Text))
                {
                    Info.Show(Localizer.Instance["UserWindow.Error2"]);
                    break;
                }
                Info1.Show(Localizer.Instance["UserWindow.Info2"]);
                res = await UserBinding.AddUser(4,
                    TextBox_Input2.Text, TextBox_Input3.Text);
                Info1.Close();
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                    break;
                }
                Info2.Show(Localizer.Instance["UserWindow.Ok2"]);
                break;
            case 5:
                server = TextBox_Input1.Text;
                if (string.IsNullOrWhiteSpace(server))
                {
                    Info.Show(Localizer.Instance["UserWindow.Error4"]);
                    break;
                }
                if (string.IsNullOrWhiteSpace(TextBox_Input2.Text) ||
                   string.IsNullOrWhiteSpace(TextBox_Input3.Text))
                {
                    Info.Show(Localizer.Instance["UserWindow.Error2"]);
                    break;
                }
                Info1.Show(Localizer.Instance["UserWindow.Info2"]);
                res = await UserBinding.AddUser(5, server,
                    TextBox_Input2.Text, TextBox_Input3.Text);
                Info1.Close();
                if (!res.Item1)
                {
                    Info.Show(res.Item2!);
                    break;
                }
                Info2.Show(Localizer.Instance["UserWindow.Ok2"]);
                TextBox_Input1.Text = "";
                break;
            default:
                Info.Show(Localizer.Instance["UserWindow.Error5"]);
                break;
        }
        Load();
        await CrossFade.Start(Grid_Add, null, CancellationToken.None);
        Button_Add.IsEnabled = true;
    }

    private void LoginOAuthCode(string url, string code)
    {
        Info1.Close();
        Info3.Show(string.Format(Localizer.Instance["UserWindow.Text3"], url),
            string.Format(Localizer.Instance["UserWindow.Text4"], code));
    }

    private void UserType_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        switch (ComboBox_UserType.SelectedIndex)
        {
            case 0:
                TextBox_Input1.IsEnabled = true;
                TextBox_Input1.Watermark = Localizer.Instance["UserWindow.Text5"];
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = false;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = false;
                TextBox_Input3.Text = "";
                break;
            case 1:
                TextBox_Input1.IsEnabled = false;
                TextBox_Input1.Watermark = "";
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = false;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = false;
                TextBox_Input3.Text = "";
                break;
            case 2:
                TextBox_Input1.IsEnabled = true;
                TextBox_Input1.Watermark = Localizer.Instance["UserWindow.Text6"];
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = true;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = true;
                TextBox_Input3.Text = "";
                break;
            case 3:
                TextBox_Input1.IsEnabled = true;
                TextBox_Input1.Watermark = Localizer.Instance["UserWindow.Text7"];
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = true;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = true;
                TextBox_Input3.Text = "";
                break;
            case 4:
                TextBox_Input1.IsEnabled = false;
                TextBox_Input1.Watermark = "";
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = true;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = true;
                TextBox_Input3.Text = "";
                break;
            case 5:
                TextBox_Input1.IsEnabled = true;
                TextBox_Input1.Watermark = Localizer.Instance["UserWindow.Text8"];
                TextBox_Input1.Text = "";
                TextBox_Input2.IsEnabled = true;
                TextBox_Input2.Text = "";
                TextBox_Input3.IsEnabled = true;
                TextBox_Input3.Text = "";
                break;
        }
    }

    private void Button_Cancel_Click(object? sender, RoutedEventArgs e)
    {
        CrossFade.Start(Grid_Add, null, CancellationToken.None);
    }

    private void Button_D1_Click(object? sender, RoutedEventArgs e)
    {
        var item = DataGrid_User.SelectedItem as UserDisplayObj;
        if (item == null)
        {
            Info.Show(Localizer.Instance["UserWindow.Error1"]);
            return;
        }

        UserBinding.Remove(item.UUID, item.AuthType);
        MainWindow.OnUserEdit();
        Load();
    }

    private void Button_S1_Click(object? sender, RoutedEventArgs e)
    {
        DataGrid_User_DoubleTapped(sender, e);
    }

    private void Button_A1_Click(object? sender, RoutedEventArgs e)
    {
        SetAdd();
    }

    private void Button_S1_PointerLeave(object? sender, PointerEventArgs e)
    {
        Expander_S.IsExpanded = false;
    }

    private void Button_S_PointerEnter(object? sender, PointerEventArgs e)
    {
        Expander_S.IsExpanded = true;
    }

    private void Button_D1_PointerLeave(object? sender, PointerEventArgs e)
    {
        Expander_D.IsExpanded = false;
    }

    private void Button_D_PointerEnter(object? sender, PointerEventArgs e)
    {
        Expander_D.IsExpanded = true;
    }

    private void Button_A1_PointerLeave(object? sender, PointerEventArgs e)
    {
        Expander_A.IsExpanded = false;
    }

    private void Button_A_PointerEnter(object? sender, PointerEventArgs e)
    {
        Expander_A.IsExpanded = true;
    }

    private void UserWindow_Opened(object? sender, EventArgs e)
    {
        DataGrid_User.MakeTran();
        Expander_A.MakePadingNull();
        Expander_D.MakePadingNull();
        Expander_S.MakePadingNull();
    }

    private void UserWindow_Closed(object? sender, EventArgs e)
    {
        App.UserWindow = null;
        CoreMain.LoginOAuthCode = null;
    }

    public void SetAdd()
    {
        CrossFade.Start(null, Grid_Add, CancellationToken.None);
    }

    public void Load()
    {
        var item1 = UserBinding.GetLastUser();
        List.Clear();
        foreach (var item in UserBinding.GetAllUser())
        {
            List.Add(new()
            {
                Name = item.Value.UserName,
                UUID = item.Key.Item1,
                AuthType = item.Key.Item2,
                Type = item.Key.Item2.GetName(),
                Text1 = item.Value.Text1,
                Text2 = item.Value.Text2,
                Use = item1 == item.Value
            });
        }
    }
}