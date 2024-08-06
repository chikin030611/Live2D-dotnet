using System.Reflection;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;

namespace ColorMC.Core.Helpers;

/// <summary>
/// 语言文件
/// </summary>
public static class LanguageHelper
{
    /// <summary>
    /// 语言储存
    /// </summary>
    private static readonly Language s_language = new();
    /// <summary>
    /// 语言类型
    /// </summary>
    private static LanguageType s_nowType;

    /// <summary>
    /// 加载语言文件
    /// </summary>
    public static void Load(LanguageType type)
    {
        string name = type switch
        {
            LanguageType.en_us => "ColorMC.Core.Resources.Language.core_en-us.json",
            _ => "ColorMC.Core.Resources.Language.core_zh-cn.json"
        };
        var assm = Assembly.GetExecutingAssembly();
        using var istr = assm.GetManifestResourceStream(name)!;
        var reader = new StreamReader(istr);
        s_language.Load(reader.ReadToEnd());
    }

    /// <summary>
    /// 取语言
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Get(string input)
    {
        return s_language.GetLanguage(input);
    }
}
