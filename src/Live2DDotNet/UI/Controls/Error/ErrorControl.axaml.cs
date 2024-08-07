using System;
using Avalonia.Media.Imaging;
using Live2DDotNet.Manager;
using Live2DDotNet.UI.Model;
using Live2DDotNet.UI.Model.Error;
using Live2DDotNet;
using Live2DDotNet.UI.Controls;

namespace Live2DDotNet.UI.Controls.Error;

public partial class ErrorControl : BaseUserControl
{
    private readonly string? _data;
    private readonly Exception? _e;
    private readonly string _e1;
    private readonly bool _close;
    private readonly bool _type = false;

    public ErrorControl()
    {
        InitializeComponent();

        UseName = ToString() ?? "ErrorControl";
    }

    public ErrorControl(string? data, Exception? e, bool close) : this()
    {
        _data = data;
        _e = e;
        _close = close;
        _type = true;

        Title = data ?? "Run Error";
    }

    public ErrorControl(string data, string e, bool close) : this()
    {
        _data = data;
        _e1 = e;
        _close = close;

        Title = data;
    }

    public override void Opened()
    {
        Window.SetTitle(Title);
    }

    public override void Closed()
    {
        if ((DataContext as ErrorModel)!.NeedClose
            || (App.IsHide))
        {
            App.Close();
        }
    }

    public override void SetModel(BaseModel model)
    {
        if (_type)
        {
            DataContext = new ErrorModel(model, _data, _e, _close);
        }
        else
        {
            DataContext = new ErrorModel(model, _data ?? "", _e1, _close);
        }
    }

}