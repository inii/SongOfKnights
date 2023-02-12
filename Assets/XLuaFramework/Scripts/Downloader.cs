using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 下载器 工具类
/// </summary>
public class Downloader : Singleton<Downloader>
{
    /// <summary>
    /// 根据模块的配置，下载对应的模块
    /// </summary>
    /// <param name="moduleConfig">模块的配置对象</param>
    /// <param name="action"> 下载完后的回调函数，回调参数表述下载是否成功 </param>
    public void Download(ModuleConfig moduleConfig, Action<bool> action)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
