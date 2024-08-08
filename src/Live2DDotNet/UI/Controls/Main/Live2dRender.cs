﻿using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Rendering;
using Avalonia.Threading;
using Live2DDotNet.UI.Flyouts;
using Live2DDotNet.UI.Model.Main;
using Live2DCSharpSDK.App;
using Live2DCSharpSDK.Framework.Motion;
using Live2DDotNet.Utils;
using Live2DDotNet.Avatar;

namespace Live2DDotNet.UI.Controls.Main;

public class Live2dRender : OpenGlControlBase, ICustomHitTest
{
    private LAppDelegate _lapp;
    private LAppModel _model;

    private DateTime _time;
    private bool _change;
    private bool _delete;
    private bool _init = false;
    private bool _first = false;

    public bool HaveModel
    {
        get
        {
            if (_lapp == null)
            {
                return false;
            }
            return _lapp.Live2dManager.GetModelNum() != 0;
        }
    }

    public Live2dRender()
    {
        DataContextChanged += Live2dRender_DataContextChanged;

        PointerPressed += Live2dTop_PointerPressed;
        PointerReleased += Live2dTop_PointerReleased;
        PointerMoved += Live2dTop_PointerMoved;
    }

    private void Live2dTop_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!HaveModel)
        {
            return;
        }

        LongPressed.Cancel();

        var pro = e.GetCurrentPoint(this);
        if (pro.Properties.IsLeftButtonPressed)
            Moved((float)pro.Position.X, (float)pro.Position.Y);
    }

    private void Flyout()
    {
        Dispatcher.UIThread.Post(() =>
        {
            _ = new Live2DFlyout(this);
        });
    }

    private void Live2dTop_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!HaveModel)
        {
            return;
        }

        LongPressed.Released();
        Release();
    }

    private void Live2dTop_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!HaveModel)
        {
            return;
        }

        var pro = e.GetCurrentPoint(this);
        if (pro.Properties.IsLeftButtonPressed)
        {
            Pressed();
            Moved((float)pro.Position.X, (float)pro.Position.Y);
        }
        else if (pro.Properties.IsRightButtonPressed)
        {
            Flyout();
        }
        else
        {
            LongPressed.Pressed(Flyout);
        }
    }

    private void Live2dRender_DataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainModel model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }
    }

    private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "ModelChange")
        {
            _change = true;
            Dispatcher.UIThread.Post(RequestNextFrameRendering);
        }
        else if (e.PropertyName == "ModelDelete")
        {
            _delete = true;
        }
    }

    private void ChangeModel()
    {
        _lapp.Live2dManager.ReleaseAllModel();
        var model = GuiConfigUtils.Config.Live2D.Model;
        if (!GuiConfigUtils.Config.Live2D.Enable || string.IsNullOrWhiteSpace(model))
        {
            return;
        }
        if (!File.Exists(model))
        {
            (DataContext as MainModel)!.Model.Show("Live2D model does not exist");
            return;
        }
        var info = new FileInfo(model);
        try
        {
            _model = _lapp.Live2dManager.LoadModel(info.DirectoryName! + "/", info.Name.Replace(".model3.json", ""));
        }
        catch (Exception e)
        {
            string temp = "Live2D model loading failed";
            Logs.Error(temp, e);
            (DataContext as MainModel)!.Model.Show(temp);
        }
    }

    private static void CheckError(GlInterface gl)
    {
        int err;
        while ((err = gl.GetError()) != GlConsts.GL_NO_ERROR)
            Console.WriteLine(err);
    }

    protected override unsafe void OnOpenGlInit(GlInterface gl)
    {
        if (_first)
            return;
        _first = true;
        if (_init)
            return;
        CheckError(gl);

        try
        {
            _lapp = new(new AvaloniaApi(this, gl), Logs.Info);
            _change = true;
            CheckError(gl);
            _init = true;
        }
        catch (Exception e)
        {
            (DataContext as MainModel)!.ChangeModelDone();
            Logs.Error("Live2D initialization failed", e);
        }
    }

    protected override void OnOpenGlDeinit(GlInterface GL)
    {
        _lapp?.Dispose();
        _lapp = null!;
        _init = false;
        _first = false;
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        if (!_init)
            return;
        var model = (DataContext as MainModel)!;
        if (_change)
        {
            _change = false;
            ChangeModel();
            model.ChangeModelDone();
        }
        if (_delete)
        {
            _delete = false;
            _lapp.Live2dManager.ReleaseAllModel();
            model.ChangeModelDone();
        }

        int x = (int)Bounds.Width;
        int y = (int)Bounds.Height;

        if (VisualRoot is TopLevel window)
        {
            var screen = window.RenderScaling;
            x = (int)(Bounds.Width * screen);
            y = (int)(Bounds.Height * screen);
        }

        gl.Viewport(0, 0, x, y);
        var now = DateTime.Now;
        float span = 0;
        if (_time.Ticks == 0)
        {
            _time = now;
        }
        else
        {
            span = (float)(now - _time).TotalSeconds;
            _time = now;
        }
        _lapp.Run(span);
        CheckError(gl);
    }

    public void Pressed()
    {
        _lapp.OnMouseCallBack(true);
    }

    public void Release()
    {
        _lapp.OnMouseCallBack(false);
    }

    public void Moved(float x, float y)
    {
        _lapp.OnMouseCallBack(x, y);
    }

    public List<string> GetMotions()
    {
        return _model.Motions;
    }

    public List<string> GetExpressions()
    {
        return _model.Expressions;
    }

    public void PlayMotion(string name)
    {
        _model.StartMotion(name, MotionPriority.PriorityForce);
    }

    public void PlayExpression(string name)
    {
        _model.SetExpression(name);
    }

    public bool HitTest(Point point)
    {
        return IsVisible;
    }

    public void StartSpeaking(int id)
    {
        string filePath = QnaAudioHelper.GetAudioPath(id);
        QnaAudioHelper.PlayAudio(filePath);
        _lapp.StartSpeaking(filePath);
    }
}
