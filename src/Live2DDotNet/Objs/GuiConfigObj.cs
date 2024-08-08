namespace Live2DDotNet.Objs;

public record Live2DSetting
{
    /// <summary>
    /// Live2D model address
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Display width
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Display height
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Whether it is enabled
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Display position
    /// </summary>
    public int Pos { get; set; }
}

/// <summary>
/// Gui configuration object
/// </summary>
public record GuiConfigObj
{
    /// <summary>
    /// Live2D settings
    /// </summary>
    public Live2DSetting Live2D { get; set; }
}

