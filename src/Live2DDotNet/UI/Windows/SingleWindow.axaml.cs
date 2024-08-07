using Avalonia.Controls;

namespace Live2DDotNet.UI.Windows;

public partial class SingleWindow : ABaseWindow
{
    public override ITopWindow ICon => Win;

    public SingleWindow()
    {
        InitializeComponent();

        Closed += UserWindow_Closed;
        Closing += SingleWindow_Closing;

        DataContext = Win.DataContext;

        InitBaseWindow();
    }

    private async void SingleWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        var res = await Win.Closing();
        if (res)
        {
            e.Cancel = true;
        }
    }

    private void UserWindow_Closed(object? sender, EventArgs e)
    {
        Win.Closed();
        App.Close();
    }
}