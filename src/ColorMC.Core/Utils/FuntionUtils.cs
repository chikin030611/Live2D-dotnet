using System.Text.RegularExpressions;
using ColorMC.Core.Objs;

namespace ColorMC.Core.Utils;

/// <summary>
/// 其他函数
/// </summary>
public static partial class FuntionUtils
{
    [GeneratedRegex("[^0-9]+")]
    private static partial Regex Regex1();

    [GeneratedRegex("^[a-zA-Z0-9]+$")]
    private static partial Regex Regex2();

    /// <summary>
    /// 执行内存回收
    /// </summary>
    public static void RunGC()
    {
        Task.Run(() =>
        {
            Task.Delay(1000).Wait();
            GC.Collect();
        });
    }
}
