using System;
/// <summary>
/// 模块资源加载器
/// </summary>
class AssetLoader : Singleton<AssetLoader>
{
    /// <summary>
    /// 加载模块对应的全局AssetBundle资源管理器
    /// </summary>
    /// <param name="moduleName">模块的名字</param>
    /// <param name="action">加载完成后的回调函数，回调参数表示加载是否成功</param>
    public void LoadAssetBundleConfig(string moduleName, Action<bool> action)
    {

    }

}
