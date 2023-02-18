

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 内存中的一个Bundle对象
/// </summary>
public class BundleRef 
{
    /// <summary>
    /// 这个Bundle的静态配置信息
    /// </summary>
    public BundleInfo bundleInfo;

    /// <summary>
    /// 加载到内存中的Bundle对象
    /// </summary>
    public AssetBundle bundle;

    /// <summary>
    /// 这些BundleRef对象被哪些AssetRef对象依赖
    /// </summary>
    public List<AssetRef> children;   

    /// <summary>
    /// BundleRef构造函数
    /// </summary>
    /// <param name="info">配置信息</param>
    public BundleRef(BundleInfo info) {
        bundleInfo = info;
    }
}
