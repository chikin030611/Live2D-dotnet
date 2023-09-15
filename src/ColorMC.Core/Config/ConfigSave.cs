﻿using ColorMC.Core.Helpers;
using ColorMC.Core.Objs.Config;
using ColorMC.Core.Utils;
using ICSharpCode.SharpZipLib.Core;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace ColorMC.Core.Config;

/// <summary>
/// 配置文件保存
/// </summary>
public static class ConfigSave
{
    private static readonly ConcurrentBag<ConfigSaveObj> s_saveQue = new();

    private static Thread t_thread;
    private static bool s_run;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        ColorMCCore.Stop += Stop;

        t_thread = new(Run)
        {
            Name = "ColorMC_Save"
        };
        s_run = true;
        t_thread.Start();
    }

    /// <summary>
    /// 停止
    /// </summary>
    private static void Stop()
    {
        s_run = false;

        Save();
    }

    /// <summary>
    /// 开始执行
    /// </summary>
    private static void Run()
    {
        int count = 0;
        while (s_run)
        {
            Thread.Sleep(100);
            if (count < 10)
            {
                count++;
                continue;
            }

            count = 0;
            if (s_saveQue.IsEmpty)
            {
                continue;
            }

            Save();
        }
    }

    private static void Save()
    {
        Dictionary<string, ConfigSaveObj> list = new();
        lock (s_saveQue)
        {
            while (s_saveQue.TryTake(out var item))
            {
                if (!list.TryAdd(item.Name, item))
                {
                    list[item.Name] = item;
                }
            }
            s_saveQue.Clear();
        }

        foreach (var item in list.Values)
        {
            try
            {
                PathHelper.WriteText(item.Local,
                    JsonConvert.SerializeObject(item.Obj, Formatting.Indented));
            }
            catch (Exception e)
            {
                Logs.Error(LanguageHelper.Get("Core.Config.Error2"), e);
            }
        }
    }

    /// <summary>
    /// 添加保存项目
    /// </summary>
    /// <param name="obj"></param>
    public static void AddItem(ConfigSaveObj obj)
    {
        lock (s_saveQue)
        {
            s_saveQue.Add(obj);
        }
    }
}