using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 内存中的单个资源对象
/// </summary>
public class AssetRef
{
    /// <summary>
    /// 资源的静态配置
    /// </summary>
    public AssetInfo assetInfo;

    /// <summary>
    /// 这个资源所属的BundleRef对象
    /// </summary>
    public BundleRef bundleRef;

    /// <summary>
    /// 这个资源所依赖的BundleRef对象列表
    /// </summary>
    public BundleRef[] dependencies;

    /// <summary>
    /// 从bundle文件提取出来的资源对象
    /// </summary>
    public Object asset;

    /// <summary>
    /// 这个资源是否是prefab
    /// </summary>
    public bool isGameObject;

    /// <summary>
    /// 这个AssetRef对象被哪些GameObject依赖
    /// </summary>
    public List<GameObject> children;

    public AssetRef(AssetInfo assetInfo) { this.assetInfo = assetInfo; }
}
