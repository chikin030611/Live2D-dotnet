using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ColorMC.Gui.Manager;
using ColorMC.Gui.UI.Animations;
using ColorMC.Gui.UI.Model;
using ColorMC.Gui.UI.Model.Main;
using ColorMC.Gui.UIBinding;
using ColorMC.Gui.Utils;

namespace ColorMC.Gui.UI.Controls.Main;

public partial class MainControl : BaseUserControl
{
    public readonly SelfPageSlideSide SidePageSlide300 = new(TimeSpan.FromMilliseconds(300));

    public MainControl()
    {
        InitializeComponent();

        Title = "ColorMC";
        UseName = ToString() ?? "MainControl";

        AddHandler(DragDrop.DragEnterEvent, DragEnter);
        AddHandler(DragDrop.DragLeaveEvent, DragLeave);
        AddHandler(DragDrop.DropEvent, Drop);

        SizeChanged += MainControl_SizeChanged;
        BaseBinding.LoadDone += LoadDone;
    }


    private void MainControl_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var config = GuiConfigUtils.Config.Live2D;
        if (DataContext is MainModel model)
        {
            model.Live2dWidth = (int)(Bounds.Width * ((float)config.Width / 100));
            model.Live2dHeight = (int)(Bounds.Height * ((float)config.Height / 100));
        }
    }

    private void DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(BaseBinding.DrapType))
        {
            return;
        }
        if (e.Data.Contains(DataFormats.Text))
        {
            Grid2.IsVisible = true;
            Label1.Text = App.Lang("UserWindow.Text8");
        }
        else if (e.Data.Contains(DataFormats.Files))
        {
            var files = e.Data.GetFiles();
            if (files == null || files.Count() > 1)
                return;

            var item = files.ToList()[0];
            if (item == null)
                return;
            if (item is IStorageFolder forder && Directory.Exists(forder.GetPath()))
            {
                Grid2.IsVisible = true;
                Label1.Text = App.Lang("AddGameWindow.Text2");
            }
            else if (item.Name.EndsWith(".zip") || item.Name.EndsWith(".mrpack"))
            {
                Grid2.IsVisible = true;
                Label1.Text = App.Lang("MainWindow.Text25");
            }
        }
    }

    private void DragLeave(object? sender, DragEventArgs e)
    {
        Grid2.IsVisible = false;
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(BaseBinding.DrapType))
        {
            return;
        }
        Grid2.IsVisible = false;
        if (e.Data.Contains(DataFormats.Text))
        {
            var str = e.Data.GetText();
            if (str == null)
            {
                return;
            }
            else if (str.StartsWith("cloudkey:") || str.StartsWith("cloudKey:"))
            {
                BaseBinding.SetCloudKey(str);
            }
        }
        else if (e.Data.Contains(DataFormats.Files))
        {
            var files = e.Data.GetFiles();
            if (files == null || files.Count() > 1)
                return;

            var item = files.ToList()[0];
            if (item == null)
                return;
        }
    }


    public override void WindowStateChange(WindowState state)
    {
        (DataContext as MainModel)!.Render = state != WindowState.Minimized;
    }

    public override void Closed()
    {
        WindowManager.MainWindow = null;

        App.Close();
    }

    public override async void Opened()
    {
        Window.SetTitle(Title);

        ChangeLive2DSize();

        if (BaseBinding.NewStart)
        {
            MainView.Opacity = 0;
            var con1 = new MainStartControl();
            Start.Child = con1;
            Start.IsVisible = true;
            await con1.Start();
            await App.CrossFade300.Start(Start, MainView, CancellationToken.None);
            Start.IsVisible = false;
        }

        if (ColorMCGui.IsCrash)
        {
            var model = (DataContext as MainModel)!;
            model.Model.Show(App.Lang("MainWindow.Error2"));
        }
    }

    public override async Task<bool> Closing()
    {
        var model = (DataContext as MainModel)!;
        if (model.IsLaunch)
        {
            var res = await model.Model.ShowWait(App.Lang("MainWindow.Info34"));
            if (res)
            {
                return false;
            }
            return true;
        }

        return false;
    }

    public void LoadDone()
    {
        Dispatcher.UIThread.Post(() =>
        {
            (DataContext as MainModel)!.LoadDone();
        });
    }

    public void LoadMain()
    {
        Dispatcher.UIThread.Post(() =>
        {
        });
    }

    public void MotdLoad()
    {
        Dispatcher.UIThread.Post(() =>
        {
            (DataContext as MainModel)!.LoadMotd();
        });
    }

    public void IsDelete()
    {
        Dispatcher.UIThread.Post(() =>
        {
        });
    }

    public void ChangeModel()
    {
        (DataContext as MainModel)!.ChangeModel();
    }

    public void DeleteModel()
    {
        (DataContext as MainModel)!.DeleteModel();
    }

    public void ChangeLive2DSize()
    {
        var config = GuiConfigUtils.Config.Live2D;
        var model = (DataContext as MainModel)!;
        model.Live2dWidth = (int)(Bounds.Width * ((float)config.Width / 100));
        model.Live2dHeight = (int)(Bounds.Height * ((float)config.Height / 100));
        model.L2dPos = (HorizontalAlignment)((config.Pos % 3) + 1);
        model.L2dPos1 = (VerticalAlignment)((config.Pos / 3) + 1);
    }

    public void ChangeLive2DMode()
    {
        var config = GuiConfigUtils.Config.Live2D;
        var model = (DataContext as MainModel)!;

        model.LowFps = config.LowFps;
    }

    public void ShowMessage(string message)
    {
        (DataContext as MainModel)!.ShowMessage(message);
    }

    public override void SetModel(BaseModel model)
    {
        var amodel = new MainModel(model);
        amodel.PropertyChanged += Amodel_PropertyChanged;
        DataContext = amodel;

        var config = GuiConfigUtils.Config.Live2D;
        amodel.Live2dWidth = (int)(Bounds.Width * ((float)config.Width / 100));
        amodel.Live2dHeight = (int)(Bounds.Height * ((float)config.Height / 100));
    }

    private void Amodel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == MainModel.SwitchView)
        {
            // SwitchView();
        }
        else if (e.PropertyName == TopModel.MinModeName)
        {
            if (DataContext is MainModel model)
            {
                if (model.MinMode)
                {
                    //HeadTop.Children.Remove(Buttons);
                    //ContentTop.Children.Add(Buttons);
                    //TopRight.IsVisible = false;

                    //TopRight.Child = null;
                    // ContentTop.Children.Add(HeadButton);
                    //HeadButton.Margin = new(0, 0, 0, 10);

                    // Right.Child = null;
                    // ContentTop.Children.Add(RightSide);
                    model.SideDisplay = false;
                }
                else
                {
                    //ContentTop.Children.Remove(Buttons);
                    //HeadTop.Children.Add(Buttons);
                    //TopRight.IsVisible = true;

                    // ContentTop.Children.Remove(HeadButton);
                    //TopRight.Child = HeadButton;
                    //HeadButton.Margin = new(0);

                    // ContentTop.Children.Remove(RightSide);
                    // Right.Child = RightSide;
                    if (!model.NewsDisplay)
                    {
                        model.SideDisplay = true;
                    }
                }
            }
        }
    }

    public override Bitmap GetIcon()
    {
        return ImageManager.GameIcon;
    }

    public void IconChange(string uuid)
    {
        if (DataContext is MainModel model)
        {
            //model.IconChange(uuid);
        }
    }
}
