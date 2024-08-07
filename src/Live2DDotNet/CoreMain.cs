using Live2DDotNet.Config;
using Live2DDotNet.Utils;

namespace Live2DDotNet;

public static class Live2DDotNetCore
{
    /// <summary>
    /// 运行路径
    /// </summary>
    public static string BaseDir { get; private set; }

    /// <summary>
    /// <summary>
    /// 错误显示回调
    /// 标题 错误 关闭程序
    /// </summary>
    public static event Action<string?, Exception?, bool>? Error;
    /// 是否为新运行
    /// </summary>
    public static bool NewStart { get; internal set; }

    /// <summary>
    /// 停止事件
    /// </summary>
    internal static event Action? Stop;

    /// <summary>
    /// 初始化阶段1
    /// </summary>
    /// <param name="dir">运行的路径</param>
    public static void Init(string Local)
    {
        if (string.IsNullOrWhiteSpace(Local))
        {
            throw new Exception("Local is empty");
        }

        BaseDir = Local;
        Directory.CreateDirectory(BaseDir);

        Logs.Init(BaseDir);

        Logs.Info("Live2DDotNet core is initializing");
    }

    /// <summary>
    /// 初始化阶段2
    /// </summary>
    public static void Init1()
    {
        ConfigSave.Init();

        Logs.Info("End of Live2DDotNet core initialization");
    }

    /// <summary>
    /// 执行关闭操作
    /// </summary>
    public static void Close()
    {
        Stop?.Invoke();
    }

    /// <summary>
    /// 启动器产生错误
    /// </summary>
    /// <param name="text"></param>
    /// <param name="e"></param>
    /// <param name="close"></param>
    public static void OnError(string text, Exception? e, bool close)
    {
        Error?.Invoke(text, e, close);
        Logs.Error(text, e);
    }

}