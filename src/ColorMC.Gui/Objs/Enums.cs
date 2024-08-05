namespace ColorMC.Gui.Objs;

/// <summary>
/// 设置类型
/// </summary>
public enum SettingType
{
    /// <summary>
    /// 默认设置
    /// </summary>
    Normal,
    /// <summary>
    /// 设置Java
    /// </summary>
    SetJava,
    /// <summary>
    /// 网络设置
    /// </summary>
    Net
}

/// <summary>
/// 颜色类型
/// </summary>
public enum ColorType
{
    /// <summary>
    /// 跟随系统
    /// </summary>
    Auto,
    /// <summary>
    /// 亮色
    /// </summary>
    Light,
    /// <summary>
    /// 暗色
    /// </summary>
    Dark
}

/// <summary>
/// 运行类型
/// </summary>
public enum RunType
{
    /// <summary>
    /// 程序
    /// </summary>
    Program,
    /// <summary>
    /// 预览器
    /// </summary>
    AppBuilder,
    /// <summary>
    /// 手机
    /// </summary>
    Phone
}

public enum FrpType
{
    SakuraFrp, OpenFrp
}

public enum HeadType
{
    Head2D, Head3D_A, Head3D_B
}