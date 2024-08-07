using Avalonia.Controls;
using Live2DDotNet.UI.Controls;
using Live2DDotNet.UI.Controls;
using Live2DDotNet.UI.Windows;

namespace Live2DDotNet.UI.Windows;

public partial class MultiWindow : AMultiWindow
{
    public override HeadControl Head => HeadControl;

    public MultiWindow()
    {
        InitializeComponent();
    }

    public MultiWindow(BaseUserControl con)
    {
        InitializeComponent();

        InitMultiWindow(con);
    }

    protected override void SetChild(Control control)
    {
        MainControl.Child = control;
    }
}
