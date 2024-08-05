namespace ColorMC.Core.Objs;

/// <summary>
/// 资源来源
/// </summary>
public enum SourceType
{
    CurseForge, Modrinth, McMod, ColorMC
}

/// <summary>
/// 路径类型
/// </summary>
public enum PathType
{
    BasePath, GamePath, ModPath, ConfigPath, ShaderpacksPath, ResourcepackPath, WorldBackPath,
    SavePath, SchematicsPath, ScreenshotsPath, RunPath, DownloadPath, JavaPath, PicPath, ServerPackPath, RunDir
}

/// <summary>
/// 文件类型
/// </summary>
public enum FileType
{
    ModPack = 0, Mod, World, Shaderpack, Resourcepack, DataPacks, Schematic,
    Java, Game, Config, AuthConfig, Pic, Optifne, Skin, Music,
    Text, Live2D, Icon, Head, JavaZip, Live2DCore, Loader, InputConfig
}

/// <summary>
/// 压缩包类型
/// </summary>
public enum PackType
{
    ColorMC, CurseForge, Modrinth, MMC, HMCL, ZipPack
}

/// <summary>
/// 语言
/// </summary>
public enum LanguageType
{
    zh_cn, en_us
}

/// <summary>
/// 下载源
/// </summary>
public enum SourceLocal
{
    Offical = 0,
    BMCLAPI = 1
}

public enum AuthType
{
    /// <summary>
    /// 离线账户
    /// </summary>
    Offline,
    /// <summary>
    /// 正版登录
    /// </summary>
    OAuth,
    /// <summary>
    /// 统一通行证
    /// </summary>
    Nide8,
    /// <summary>
    /// 外置登录
    /// </summary>
    AuthlibInjector,
    /// <summary>
    /// 皮肤站
    /// </summary>
    LittleSkin,
    /// <summary>
    /// 自建皮肤站
    /// </summary>
    SelfLittleSkin
}

/// <summary>
/// 位数
/// </summary>
public enum ArchEnum
{
    x86,
    x86_64,
    aarch64,
    arm,
    unknow
}

/// <summary>
/// 系统
/// </summary>
public enum OsType
{
    Windows,
    Linux,
    MacOS,
    Android
}

/// <summary>
/// 运行态
/// </summary>
public enum CoreRunState
{
    Read, Init, GetInfo, Start, End,
    Download,
    Error,
}

/// <summary>
/// 运行态
/// </summary>
public enum DownloadState
{
    Start, End
}