using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 模块管理工具类
/// </summary>
public class ModuleManager : Singleton<ModuleManager>
{
    /// <summary>
    /// 加载一个模块，唯一对外API函数
    /// </summary>
    public async Task<bool> Load(ModuleConfig moduleConfig)
    {
        if (GlobalConfig.HotUpdate == false)
        {
            if (GlobalConfig.BundleMode == false)
            {
                return true;
            }
            else
            {
                ModuleABConfig moduleABConfig = await AssetLoader.Instance.LoadAssetBundleConfig(moduleConfig.moduleName);
                if (moduleABConfig == null)
                {
                    return false;
                }

                Debug.Log("模块包含AB包总数量：" + moduleABConfig.BundleArray.Count);
                Hashtable path2AssetRef = AssetLoader.Instance.ConfigAssembly(moduleABConfig);
                AssetLoader.Instance.base2Assets.Add(moduleConfig.moduleName, path2AssetRef);

                return true;
            }
        }
        else
        {
            return await Downloader.Instance.Download(moduleConfig);
            //Downloader.Instance.Download(moduleConfig, (downloadResult) =>
            //{
            //    if (downloadResult == true)
            //    {
            //        if (GlobalConfig.BundleMode == true)
            //        {
            //            LoadAssetBundleConfig(moduleConfig, moduleAction);
            //        }
            //        else
            //        {
            //            Debug.LogError("配置错误(config error)！ HotUpdate == true && BundleMode == false");
            //        }
            //    }
            //    else
            //    {
            //        Debug.LogError($"下载失败， Unable to download ");
            //    }
            //});
        }
    }

    //private async Task<bool> LoadAssetBundleConfig(ModuleConfig moduleConfig)
    //{
    //    AssetLoader.Instance.LoadAssetBundleConfig(moduleConfig.moduleName, (assetConfigResult) =>
    //    {
    //        if (assetConfigResult == true)
    //        {
    //            moduleAction(true);
    //        }
    //        else
    //        {
    //            Debug.LogError("LoadAssetBundleConfig出错！");
    //        }
    //    });
    //}
}
