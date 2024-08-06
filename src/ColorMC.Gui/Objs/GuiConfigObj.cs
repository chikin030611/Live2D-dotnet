namespace ColorMC.Gui.Objs;

public record Live2DSetting
{
    /// <summary>
    /// Live2D模型地址
    /// </summary>
    public string? Model { get; set; }
    /// <summary>
    /// 显示宽度
    /// </summary>
    public int Width { get; set; }
    /// <summary>
    /// 显示高度
    /// </summary>
    public int Height { get; set; }
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// 显示位置
    /// </summary>
    public int Pos { get; set; }
    /// <summary>
    /// 低帧率模式
    /// </summary>
    public bool LowFps { get; set; }
}

/// <summary>
/// Windows窗口渲染设置
/// </summary>
public record WindowsRenderSetting
{
    public bool? ShouldRenderOnUIThread { get; set; }
    public bool? OverlayPopups { get; set; }
}

/// <summary>
/// X11窗口渲染设置
/// </summary>
public record X11RenderSetting
{
    public bool? UseDBusMenu { get; set; }
    public bool? UseDBusFilePicker { get; set; }
    public bool? OverlayPopups { get; set; }
    public bool? SoftwareRender { get; set; }
}

/// <summary>
/// 渲染设置
/// </summary>
public record RenderSetting
{
    /// <summary>
    /// Windows设置
    /// </summary>
    public WindowsRenderSetting Windows { get; set; }
    /// <summary>
    /// X11设置
    /// </summary>
    public X11RenderSetting X11 { get; set; }
}

/// <summary>
/// Gui配置文件
/// </summary>
public record GuiConfigObj
{
    /// <summary>
    /// 渲染设置
    /// </summary>
    public RenderSetting Render { get; set; }
    /// Live2D设置
    /// </summary>
    public Live2DSetting Live2D { get; set; }
}

