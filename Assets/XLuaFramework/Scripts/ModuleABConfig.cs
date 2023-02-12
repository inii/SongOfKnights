using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// һ��Bundle���� ���������л�json�ļ���
/// </summary>
public class BundleInfo
{
    /// <summary>
    /// ���bundle������
    /// </summary>
    public string bundle_name;

    /// <summary>
    /// ���bundle��Դ��crcɢ�б�
    /// </summary>
    public string crc; 

    /// <summary>
    /// ���bundle����������Դ��·���б�
    /// </summary>
    public List<string> assets;
}


/// <summary>
///  һ��Asset���� ���������л�Ϊjson�ļ���
/// </summary>
public class AssetInfo
{
    /// <summary>
    /// �����Դ�����·��
    /// </summary>
    public string asset_path;

    /// <summary>
    /// �����Դ������AssetBundle������
    /// </summary>
    public string bundle_name;

    /// <summary>
    /// �����Դ��������AssetBundle�б������
    /// </summary>
    public List<string> dependencies;

}

/// <summary>
/// ModuleABConfig���� ��Ӧ��������ģ���json�ļ� 
/// </summary>
public class ModuleABConfig
{

    /// <summary>
    /// key: AssetBundle������
    /// </summary>
    public Dictionary<string, BundleInfo> BundleArray;

    public AssetInfo[] AssetArray;
    //public ModuleABConfig() { }

    public ModuleABConfig(int assetCount)
    {
        BundleArray = new Dictionary<string, BundleInfo>();
        AssetArray = new AssetInfo[assetCount];
    }

    /// <summary>
    /// ����һ��bundle��¼
    /// </summary>
    /// <param name="bundleName">bundle��id</param>
    /// <param name="bundleInfo">bundle�Ķ���</param>
    public void AddBundle(string bundleName, BundleInfo bundleInfo)
    {
        BundleArray[bundleName] = bundleInfo;
    }

    public void AddAsset(int index, AssetInfo assetInfo)
    {
        AssetArray[index] = assetInfo;
    }


}
