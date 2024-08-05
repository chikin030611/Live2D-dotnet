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
    /// 切换语言文件
    /// </summary>
    public static void Change(LanguageType type)
    {
        if (s_nowType == type)
            return;

        s_nowType = type;
        Load(type);
        ColorMCCore.OnLanguageReload(type);
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

    public static string GetName(this AuthType type)
    {
        return type switch
        {
            AuthType.Offline => Get("Type.AuthType.Offline"),
            AuthType.OAuth => Get("Type.AuthType.OAuth"),
            AuthType.Nide8 => Get("Type.AuthType.Nide8"),
            AuthType.AuthlibInjector => Get("Type.AuthType.AuthlibInjector"),
            AuthType.LittleSkin => Get("Type.AuthType.LittleSkin"),
            AuthType.SelfLittleSkin => Get("Type.AuthType.SelfLittleSkin"),
            _ => Get("Type.AuthType.Other")
        };
    }

    public static string GetName(this SourceLocal state)
    {
        return state switch
        {
            SourceLocal.Offical => Get("Type.SourceLocal.Offical"),
            SourceLocal.BMCLAPI => Get("Type.SourceLocal.BMCLAPI"),
            _ => Get("Type.SourceLocal.Other")
        };
    }

    public static string GetName(this GCType state)
    {
        return state switch
        {
            GCType.G1GC => Get("Type.GCType.G1GC"),
            GCType.SerialGC => Get("Type.GCType.SerialGC"),
            GCType.ParallelGC => Get("Type.GCType.ParallelGC"),
            GCType.CMSGC => Get("Type.GCType.CMSGC"),
            GCType.User => Get("Type.GCType.User"),
            _ => Get("Type.GCType.Other")
        };
    }

    public static string GetName(this LanguageType type)
    {
        return type switch
        {
            LanguageType.en_us => "English(AI)",
            LanguageType.zh_cn => "简体中文",
            _ => ""
        };
    }

    public static string GetName(this SourceType type)
    {
        return type switch
        {
            SourceType.CurseForge => Get("Type.SourceType.CurseForge"),
            SourceType.Modrinth => Get("Type.SourceType.Modrinth"),
            SourceType.McMod => Get("Type.SourceType.McMod"),
            _ => Get("Type.SourceType.Other")
        };
    }

    public static string GetName(this PackType type)
    {
        return type switch
        {
            PackType.ColorMC => Get("Type.PackType.ColorMC"),
            PackType.CurseForge => Get("Type.PackType.CurseForge"),
            PackType.Modrinth => Get("Type.PackType.Modrinth"),
            PackType.MMC => Get("Type.PackType.MMC"),
            PackType.HMCL => Get("Type.PackType.HMCL"),
            PackType.ZipPack => Get("Type.PackType.ZipPack"),
            _ => Get("Type.PackType.Other")
        };
    }

    public static string GetName(this FileType type)
    {
        return type switch
        {
            FileType.ModPack => Get("Type.FileType.ModPack"),
            FileType.Mod => Get("Type.FileType.Mod"),
            FileType.World => Get("Type.FileType.World"),
            FileType.Shaderpack => Get("Type.FileType.Shaderpack"),
            FileType.Resourcepack => Get("Type.FileType.Resourcepack"),
            FileType.DataPacks => Get("Type.FileType.DataPacks"),
            FileType.Optifne => Get("Type.FileType.Optifne"),
            _ => Get("Type.FileType.Other")
        };
    }
}
