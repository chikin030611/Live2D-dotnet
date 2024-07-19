using ColorMC.Core.Objs.Java;
using Newtonsoft.Json;

namespace ColorMC.Core.Net.Apis;

/// <summary>
/// Dragonwell下载源
/// </summary>
public static class Dragonwell
{
    /// <summary>
    /// 获取列表
    /// </summary>
    public static async Task<DragonwellObj?> GetJavaList()
    {
        var url = "https://dragonwell-jdk.io/releases.json";
        var data = await WebClient.DownloadClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        if (data == null)
            return null;
        var str = await data.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<DragonwellObj>(str);
    }
}
