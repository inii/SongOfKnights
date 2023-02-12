using System;
using UnityEngine;

public class ModuleManager : Singleton<ModuleManager>
{
    /// <summary>
    /// 加载一个模块，唯一对外API函数
    /// </summary>
    public void Load(ModuleConfig moduleConfig, Action<bool> moduleAction)
    {
        if (GlobalConfig.HotUpdate == false)
        {
            if (GlobalConfig.BundleMode == false)
            {
                moduleAction(true);
            }
            else
            {
                LoadAssetBundleConfig(moduleConfig, moduleAction);
            }
        }
        else
        {
            Downloader.Instance.Download(moduleConfig, (downloadResult) =>
            {
                if (downloadResult == true)
                {
                    if (GlobalConfig.BundleMode == true)
                    {
                        LoadAssetBundleConfig(moduleConfig, moduleAction);
                    }
                    else
                    {
                        Debug.LogError("配置错误(config error)！ HotUpdate == true && BundleMode == false");
                    }
                }
                else
                {
                    Debug.LogError($"下载失败， Unable to download ");
                }
            });
        }
    }

    private void LoadAssetBundleConfig(ModuleConfig moduleConfig, Action<bool> moduleAction)
    {
        AssetLoader.Instance.LoadAssetBundleConfig(moduleConfig.moduleName, (assetConfigResult) =>
        {
            if (assetConfigResult == true)
            {
                moduleAction(true);
            }
            else
            {
                Debug.LogError("LoadAssetBundleConfig出错！");
            }
        });
    }
}
