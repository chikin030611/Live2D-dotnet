using System;
using System.Timers;
using Avalonia.Controls;
using Avalonia.OpenGL.Controls;
using Timer = System.Timers.Timer;

namespace Live2DDotNet.UI;

/// <summary>
/// Fps限制器
/// </summary>
public class FpsTimer
{
    private readonly OpenGlControlBase _render;
    private TopLevel _top;
    private readonly Timer _timer;
    private bool _pause = true;
    private bool _last;

    public Action<int>? FpsTick { private get; init; }
    public bool Pause
    {
        get
        {
            return _pause;
        }
        set
        {
            //不改变
            if (_pause == value)
            {
                return;
            }
            //暂停 -> 继续
            if (_pause && value == false)
            {
                _top ??= TopLevel.GetTopLevel(_render) ?? throw new Exception();
                _pause = false;
                _timer.Start();
                Go();
            }
            else //暂停
            {
                _pause = true;
                _timer.Stop();
            }
        }
    }
    public int NowFps { get; private set; }

    public FpsTimer(OpenGlControlBase render)
    {
        _render = render;
        _timer = new(TimeSpan.FromSeconds(1));
        _timer.BeginInit();
        _timer.AutoReset = true;
        _timer.Elapsed += Timer_Elapsed;
        _timer.EndInit();
    }

    private void Go()
    {
        if (!_pause)
        {
            _top.RequestAnimationFrame((t) =>
            {
                _render.RequestNextFrameRendering();
                NowFps++;
                Go();
            });
        }
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (!Pause)
        {
            FpsTick?.Invoke(NowFps);
        }
        NowFps = 0;
    }

    public void Close()
    {
        _pause = true;
        _timer.Stop();
        _timer.Close();
        _timer.Dispose();
    }
}
