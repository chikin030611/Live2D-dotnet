using System.Collections.Concurrent;
using Live2DDotNet.Helpers;
using Live2DDotNet.Objs.Config;
using Live2DDotNet.Utils;
using Newtonsoft.Json;

namespace Live2DDotNet.Config;

/// <summary>
/// 配置文件保存
/// </summary>
public static class ConfigSave
{
    /// <summary>
    /// 保存队列
    /// </summary>
    private static readonly ConcurrentBag<ConfigSaveObj> s_saveQue = [];

    /// <summary>
    /// 保存线程
    /// </summary>
    private static Thread t_thread;
    /// <summary>
    /// 是否在运行
    /// </summary>
    private static bool s_run;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        Live2DDotNetCore.Stop += Stop;

        t_thread = new(Run)
        {
            Name = "Live2DDotNet_Save"
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

    /// <summary>
    /// 保存所有文件
    /// </summary>
    private static void Save()
    {
        var list = new Dictionary<string, ConfigSaveObj>();
        while (s_saveQue.TryTake(out var item))
        {
            if (!list.TryAdd(item.Name, item))
            {
                list[item.Name] = item;
            }
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
                Logs.Error("Error: Saving Configuration Files", e);
            }
        }
    }

    /// <summary>
    /// 添加保存项目
    /// </summary>
    /// <param name="obj"></param>
    public static void AddItem(ConfigSaveObj obj)
    {
        s_saveQue.Add(obj);
    }
}