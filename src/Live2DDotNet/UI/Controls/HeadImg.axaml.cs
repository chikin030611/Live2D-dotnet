using Avalonia;
using Avalonia.Controls;

namespace Live2DDotNet.UI.Controls;

public partial class HeadImg : Decorator
{
    /// <summary>
    /// Defines the <see cref="Path"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> PathProperty =
        AvaloniaProperty.Register<HeadImg, string?>(nameof(Path));

    public string? Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    public HeadImg()
    {
        InitializeComponent();
    }
}
