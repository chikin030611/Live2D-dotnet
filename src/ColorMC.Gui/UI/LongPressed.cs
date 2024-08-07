using System;
using System.Timers;
using Avalonia.Threading;
using ColorMC.Gui.Objs;
using ColorMC.Gui.Utils;

namespace ColorMC.Gui.UI;

/// <summary>
/// 长按处理
/// </summary>
public static class LongPressed
{
    private static readonly Timer t_timer;

    private static Action? s_action;

    static LongPressed()
    {
        t_timer = new();
        t_timer.BeginInit();
        t_timer.AutoReset = false;
        t_timer.Elapsed += Timer_Elapsed;
        t_timer.Interval = 500;
        t_timer.EndInit();

        App.OnClose += App_OnClose;
    }

    /// <summary>
    /// 开始一个长按
    /// </summary>
    /// <param name="action">运行</param>
    public static void Pressed(Action action)
    {
        s_action = action;

        t_timer.Start();
    }

    private static void App_OnClose()
    {
        s_action = null;
        t_timer.Dispose();
    }

    private static void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            s_action?.Invoke();
        });
    }

    /// <summary>
    /// 结束一个长按
    /// </summary>
    public static void Released()
    {
        Cancel();
    }

    public static void Cancel()
    {
        s_action = null;
        t_timer.Stop();
    }
}
