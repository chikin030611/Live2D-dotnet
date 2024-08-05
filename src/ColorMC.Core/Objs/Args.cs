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
