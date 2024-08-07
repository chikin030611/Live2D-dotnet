using ColorMC.Gui.Manager;
using ColorMC.Gui.Utils;

namespace ColorMC.Gui.UIBinding;

public static class ConfigBinding
{
    /// <summary>
    /// 删除Live2D模型
    /// </summary>
    public static void DeleteLive2D()
    {
        GuiConfigUtils.Config.Live2D ??= GuiConfigUtils.MakeLive2DConfig();
        GuiConfigUtils.Config.Live2D.Model = null;
        GuiConfigUtils.Save();

        WindowManager.MainWindow?.DeleteModel();
    }

    /// <summary>
    /// 设置启用Live2D模型
    /// </summary>
    /// <param name="enable"></param>
    public static void SetLive2D(bool enable)
    {
        GuiConfigUtils.Config.Live2D ??= GuiConfigUtils.MakeLive2DConfig();
        GuiConfigUtils.Config.Live2D.Enable = enable;
        GuiConfigUtils.Save();

        WindowManager.MainWindow?.ChangeModel();
    }

    /// <summary>
    /// 设置Live2D模型
    /// </summary>
    /// <param name="live2DModel"></param>
    public static void SetLive2D(string? live2DModel)
    {
        GuiConfigUtils.Config.Live2D ??= GuiConfigUtils.MakeLive2DConfig();
        GuiConfigUtils.Config.Live2D.Model = live2DModel;
        GuiConfigUtils.Save();

        WindowManager.MainWindow?.ChangeModel();
    }

    /// <summary>
    /// 设置Live2D界面大小
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="pos"></param>
    public static void SetLive2DSize(int width, int height, int pos)
    {
        GuiConfigUtils.Config.Live2D ??= GuiConfigUtils.MakeLive2DConfig();
        GuiConfigUtils.Config.Live2D.Width = width;
        GuiConfigUtils.Config.Live2D.Height = height;
        GuiConfigUtils.Config.Live2D.Pos = pos;
        GuiConfigUtils.Save();

        WindowManager.MainWindow?.ChangeLive2DSize();
    }

}
