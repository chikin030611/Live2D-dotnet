using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using ICSharpCode.SharpZipLib.Zip;
using Live2DDotNet.Helpers;
using Live2DDotNet.Manager;
using Live2DDotNet.UI;

namespace Live2DDotNet.UIBinding;

public static class BaseBinding
{

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        Live2DDotNetCore.Error += WindowManager.ShowError;

        InputElement.PointerReleasedEvent.AddClassHandler<DataGridCell>((x, e) =>
        {
            LongPressed.Released();
        }, handledEventsToo: true);
    }

    /// <summary>
    /// 导入Live2D核心
    /// </summary>
    /// <param name="local"></param>
    /// <returns></returns>
    public static async Task<bool> SetLive2DCore(string local)
    {
        using var stream = PathHelper.OpenRead(local);
        using var zip = new ZipFile(stream);
        string file = "";
        string file1 = Directory.GetCurrentDirectory();
        file = "Core/dll/windows/" + "x86_64" + "/Live2DCubismCore.dll";
        file1 += "/Live2DCubismCore.dll";

        file1 = Path.GetFullPath(file1);

        foreach (ZipEntry item in zip)
        {
            if (item.IsFile && item.Name.Contains(file))
            {
                using var stream1 = zip.GetInputStream(item);
                using var stream2 = PathHelper.OpenWrite(file1);
                await stream1.CopyToAsync(stream2);
                return true;
            }
        }

        return false;
    }

}
