﻿using ColorMC.Core.Objs.Minecraft;
using Newtonsoft.Json;

namespace ColorMC.Core.Net;

public static class Get
{
    public static async Task<AssetsObj?> GetAssets(string url)
    {
        try
        {
            var data = await BaseClient.GetString(url);
            if (string.IsNullOrWhiteSpace(data))
                return null;
            return JsonConvert.DeserializeObject<AssetsObj>(data);
        }
        catch (Exception e)
        {
            Logs.Error("获取游戏资源信息发生错误", e);
            return null;
        }
    }

    public static async Task<GameArgObj?> GetGame(string url)
    {
        try
        {
            var data = await BaseClient.GetString(url);
            if (string.IsNullOrWhiteSpace(data))
                return null;
            return JsonConvert.DeserializeObject<GameArgObj>(data);
        }
        catch (Exception e)
        {
            Logs.Error("获取游戏信息发生错误", e);
            return null;
        }
    }


    public static async Task<VersionObj?> GetVersions(SourceLocal? local = null)
    {
        try
        {
            var data = await BaseClient.GetString(UrlHelp.GameVersion(local));
            if (string.IsNullOrWhiteSpace(data))
                return null;
            return JsonConvert.DeserializeObject<VersionObj>(data);
        }
        catch (Exception e)
        {
            Logs.Error("获取版本信息发生错误", e);
            return null;
        }
    }
}