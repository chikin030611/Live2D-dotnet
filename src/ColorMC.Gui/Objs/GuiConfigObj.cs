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
}

/// <summary>
/// Gui配置文件
/// </summary>
public record GuiConfigObj
{
    /// Live2D设置
    /// </summary>
    public Live2DSetting Live2D { get; set; }
}

