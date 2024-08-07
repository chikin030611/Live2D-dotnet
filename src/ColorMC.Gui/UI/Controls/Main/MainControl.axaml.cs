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
using ColorMC.Gui.UI.Model;
using ColorMC.Gui.UI.Model.Main;
using ColorMC.Gui.UIBinding;
using ColorMC.Gui.Utils;

namespace ColorMC.Gui.UI.Controls.Main;

public partial class MainControl : BaseUserControl
{

    public MainControl()
    {
        InitializeComponent();

        Title = "Live2D.NET";
        UseName = ToString() ?? "MainControl";

        SizeChanged += MainControl_SizeChanged;
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

    public override void WindowStateChange(WindowState state)
    {
        (DataContext as MainModel)!.Render = state != WindowState.Minimized;
    }

    public override void Closed()
    {
        WindowManager.MainWindow = null;

        App.Close();
    }

    public override void Opened()
    {
        Window.SetTitle(Title);

        ChangeLive2DSize();

        if (ColorMCGui.IsCrash)
        {
            var model = (DataContext as MainModel)!;
            model.Model.Show("Error running launcher update, back to old version");
        }
    }

    public override async Task<bool> Closing()
    {
        var model = (DataContext as MainModel)!;
        if (model.IsLaunch)
        {
            var res = await model.Model.ShowWait("Game is running. Turn off launcher");
            if (res)
            {
                return false;
            }
            return true;
        }

        return false;
    }

    public void LoadMain()
    {
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
        if (e.PropertyName == TopModel.MinModeName)
        {
            if (DataContext is MainModel model)
            {
                if (model.MinMode)
                {
                    model.SideDisplay = false;
                }
                else
                {
                    if (!model.NewsDisplay)
                    {
                        model.SideDisplay = true;
                    }
                }
            }
        }
    }
}
