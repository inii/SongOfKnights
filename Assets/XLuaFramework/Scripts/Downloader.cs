using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

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
    public async Task<bool> Download(ModuleConfig moduleConfig)
    {
        UnityWebRequest request = UnityWebRequest.Get("test_url");
        await request.SendWebRequest();

        if (string.IsNullOrEmpty(request.error))
        {
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}



