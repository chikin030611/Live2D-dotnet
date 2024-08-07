using System.Text;
using System.IO;
using ColorMC.Gui.Objs;
using ColorMC.Gui.Utils;

namespace ColorMC.Gui.Helpers;

/// <summary>
/// 文件与路径处理
/// </summary>
public static class PathHelper
{
    /// <summary>
    /// 读文件
    /// </summary>
    /// <param name="local">路径</param>
    /// <returns>流</returns>
    public static Stream? OpenRead(string local)
    {
        if (File.Exists(local))
        {
            return File.Open(local, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        return null;
    }

    /// <summary>
    /// 写文件
    /// </summary>
    /// <param name="local">路径</param>
    /// <returns>流</returns>
    public static Stream OpenWrite(string local)
    {
        var info = new FileInfo(local);
        info.Directory?.Create();
        return File.Open(local, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
    }

    /// <summary>
    /// 写文本
    /// </summary>
    /// <param name="local">路径</param>
    /// <param name="str">数据</param>
    public static void WriteText(string local, string str)
    {
        var data = Encoding.UTF8.GetBytes(str);
        WriteBytes(local, data);
    }

    /// <summary>
    /// 写文件
    /// </summary>
    /// <param name="local">路径</param>
    /// <param name="data">数据</param>
    public static void WriteBytes(string local, byte[] data)
    {
        var info = new FileInfo(local);
        info.Directory?.Create();
        using var stream = OpenWrite(local);
        stream.Write(data, 0, data.Length);
    }
}