using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ColorMC.Core;
using ColorMC.Core.Helpers;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Manager;

namespace ColorMC.Gui.Utils;

/// <summary>
/// 启动器更新器
/// </summary>
public static class UpdateChecker
{
    public static readonly string[] WebSha1s = ["", "", "", ""];
    public static readonly string[] Sha1s = ["", "", "", ""];
    public static readonly string[] LocalPath = ["", "", "", ""];

    public static void Init()
    {
        if (ColorMCGui.BaseSha1 == null)
        {
            return;
        }

        LocalPath[0] = Path.GetFullPath($"{ColorMCGui.RunDir}dll/ColorMC.Core.dll");
        LocalPath[1] = Path.GetFullPath($"{ColorMCGui.RunDir}dll/ColorMC.Core.pdb");
        LocalPath[2] = Path.GetFullPath($"{ColorMCGui.RunDir}dll/ColorMC.Gui.dll");
        LocalPath[3] = Path.GetFullPath($"{ColorMCGui.RunDir}dll/ColorMC.Gui.pdb");

        for (int a = 0; a < 4; a++)
        {
            if (File.Exists(LocalPath[a]))
            {
                using var file = PathHelper.OpenRead(LocalPath[a])!;
                Sha1s[a] = HashHelper.GenSha1(file);
            }
            else
            {
                Sha1s[a] = ColorMCGui.BaseSha1[a];
            }
        }
    }

    public static async Task<(bool, bool, string?)> Check()
    {
        if (ColorMCGui.BaseSha1 == null)
        {
            return (false, false, null);
        }


        return (false, false, null);
    }

    public static void UpdateCheckFail()
    {
        var window = WindowManager.GetMainWindow();
        if (window == null)
        {
            return;
        }
        window.Model.Show(App.Lang("SettingWindow.Tab3.Error2"));
    }
}
