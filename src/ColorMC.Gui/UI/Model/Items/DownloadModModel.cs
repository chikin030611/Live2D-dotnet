﻿using System.Collections.Generic;
using ColorMC.Gui.Objs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ColorMC.Gui.UI.Model.Items;

/// <summary>
/// Mod下载项目显示
/// </summary>
public partial class DownloadModModel : ObservableObject
{
    [ObservableProperty]
    private bool _download;

    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; set; }
    public int SelectVersion { get; set; }

    public List<string> Version => ModVersion;

    /// <summary>
    /// 是否为可选
    /// </summary>
    public bool Optional;
    /// <summary>
    /// 版本列表
    /// </summary>
    public List<string> ModVersion;
    /// <summary>
    /// 下载项目列表
    /// </summary>
    public List<DownloadModArg> Items;
}
