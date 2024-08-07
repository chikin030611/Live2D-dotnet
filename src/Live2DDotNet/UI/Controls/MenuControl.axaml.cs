using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Live2DDotNet.UI.Model;

namespace Live2DDotNet.UI.Controls;

public abstract partial class MenuControl : BaseUserControl
{
    private CancellationTokenSource _cancel = new();
    private CancellationTokenSource _cancel1 = new();

    private bool _switch1 = false;

    private readonly BaseMenuControl _control;

    private int _now = -1;

    public MenuControl()
    {
        _control = new();

        DataContextChanged += MenuControl_DataContextChanged;
        SizeChanged += MenuControl_SizeChanged;

        _control.TabPanel.SizeChanged += TabPanel_SizeChanged;

        Content = _control;
    }

    private void TabPanel_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (DataContext is TopModel model)
        {
            model.WidthChange(0, e.NewSize.Width);
        }
    }

    protected abstract Control ViewChange(int old, int index);

    private void MenuControl_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var model = (DataContext as MenuModel)!;
        if (e.NewSize.Width > 500)
        {
            model.TopSide = false;
            _control.SidePanel2.Child = null;
            _control.TopPanel.Margin = new Thickness(0);
        }
        else
        {
            model.TopSide = true;
            _control.TopPanel.Margin = new Thickness(10, 0, 0, 0);
        }
    }

    private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == MenuModel.SideOpen)
        {
            _cancel1.Cancel();
            _cancel1.Dispose();
            _cancel1 = new();

            _control.SidePanel3.IsVisible = true;
            Dispatcher.UIThread.Post(() =>
            {
            });
        }
        else if (e.PropertyName == MenuModel.SideClose)
        {
            _cancel1.Cancel();
            _cancel1.Dispose();
            _cancel1 = new();
            _control.SidePanel3.IsVisible = false;
        }

        if (e.PropertyName == MenuModel.NowViewName)
        {
            var model = (DataContext as MenuModel)!;
            Go(ViewChange(_now, model.NowView));
            _now = model.NowView;
        }
    }

    private void Go(Control to)
    {
        _cancel.Cancel();
        _cancel.Dispose();

        _cancel = new();

        var model = (DataContext as MenuModel)!;

        if (_now == -1)
        {
            _control.Content1.Child = to;
            return;
        }

        _control.SwitchTo(_switch1, to, _now < model.NowView, _cancel.Token);

        _switch1 = !_switch1;
    }

    private void MenuControl_DataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MenuModel model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }
    }
}

public partial class BaseMenuControl : UserControl
{
    public BaseMenuControl()
    {
        InitializeComponent();
    }

    public void SwitchTo(bool dir, Control control, bool dir1, CancellationToken token)
    {
        if (!dir)
        {
            Content2.Child = control;
        }
        else
        {
            Content1.Child = control;
        }
    }
}