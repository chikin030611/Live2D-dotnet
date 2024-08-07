using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Live2DDotNet.UI.Model;
using Live2DDotNet.Manager;
using Live2DDotNet.UI.Windows;

namespace Live2DDotNet.UI.Controls;

public abstract class BaseUserControl : UserControl, ITopWindow
{
    public BaseUserControl()
    {
        SizeChanged += BaseUserControl_SizeChanged;
    }

    private void BaseUserControl_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (DataContext is TopModel model)
        {
            model.WidthChange(-1, e.NewSize.Width);
        }
    }

    public IBaseWindow Window => WindowManager.FindRoot(VisualRoot);
    public string Title { get; protected set; }
    public string UseName { get; protected set; }
    public void SetBaseModel(BaseModel model)
    {
        SetModel(model);
        if (DataContext is TopModel model1)
        {
            model1.PropertyChanged += Model_PropertyChanged;
        }
    }

    private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == TopModel.WindowCloseName)
        {
            Window.Close();
        }
    }

    public abstract void SetModel(BaseModel model);
    public virtual void WindowStateChange(WindowState state) { }
    public virtual Task<bool> OnKeyDown(object? sender, KeyEventArgs e)
    {
        return Task.FromResult(false);
    }
    public virtual void IPointerPressed(PointerPressedEventArgs e) { }
    public virtual void IPointerReleased(PointerReleasedEventArgs e) { }
    public virtual void Opened() { }
    public virtual void Closed() { }
    public virtual void Update() { }
    public virtual Task<bool> Closing() { return Task.FromResult(false); }
    public void Close()
    {
        if (DataContext is TopModel model)
        {
            model.WindowClose();
        }
    }
}
