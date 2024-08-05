namespace ColorMC.Core.Objs;

public record CoreInitArg
{
    /// <summary>
    /// 运行的路径
    /// </summary>
    public string Local;
    public string? OAuthKey;
    public string? CurseForgeKey;
}

/// <summary>
/// 下载Gui调用所需参数
/// </summary>
public record DownloadArg
{
    /// <summary>
    /// 下载状态更新
    /// </summary>
    public ColorMCCore.DownloadUpdate? Update;
    /// <summary>
    /// 下载任务更新
    /// </summary>
    public ColorMCCore.DownloadTaskUpdate? UpdateTask;
}

/// <summary>
/// 删除文件夹参数
/// </summary>
public record DeleteFilesArg
{
    /// <summary>
    /// 路径
    /// </summary>
    public required string Local;
    /// <summary>
    /// 请求回调
    /// </summary>
    public ColorMCCore.Request? Request;
}

public abstract record UnpackGameZipArg
{
    public string? Name;
    public string? Group;
    public ColorMCCore.ZipUpdate? Zip;
    public ColorMCCore.Request? Request;
    public ColorMCCore.PackUpdate? Update;
    public ColorMCCore.PackState? Update2;
}
