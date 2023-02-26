using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

/**
 *   不管是win还是Android或者ios，都有两个资源路径
 *   1.app安装时生产的资源只读路径Base，（unity中是以Application.streamingAssetsPath开头的路径)
 *   2.另一个是app提供的可读写路径Updata, (unity中是以Application.persistentDataPath开头的路径)
 *   
 *   对于资源的加载，我们要有限查找Updata，如果存在直接加载返回，否则查找Base
 **/


/// <summary>
/// 模块资源加载器
/// </summary>
class AssetLoader : Singleton<AssetLoader>
{
    /// <summary>
    /// 平台对应的只读路径下的资源
    /// Key 模块名字
    /// Value 模块所有的资源
    /// </summary>
    public Dictionary<string, Hashtable> base2Assets;

    /// <summary>
    /// 平台对应的可读写路径
    /// </summary>
    public Dictionary<string, Hashtable> update2Assets;

    /// <summary>
    /// 记录所有的BundleRef(不管是Base还是Update路径)
    /// 键是bundle的名字
    /// </summary>
    public Dictionary<string, BundleRef> name2BundleRef;

    /// <summary>
    /// 模块资源加载器的构造函数
    /// </summary>
    public AssetLoader()
    {
        base2Assets = new Dictionary<string, Hashtable>();
        update2Assets = new Dictionary<string, Hashtable>();
        name2BundleRef = new Dictionary<string, BundleRef>();
    }

    /// <summary>
    /// 加载模块对应的全局AssetBundle资源管理器
    /// </summary>
    /// <param name="moduleName">模块的名字</param>
    public async Task<ModuleABConfig> LoadAssetBundleConfig(BaseOrUpdate baseOrUpdate, string moduleName, string bundleConfigName)
    {
        string url = BundlePath(baseOrUpdate, moduleName, bundleConfigName);
        UnityWebRequest request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (string.IsNullOrEmpty(request.error) == true)
        {
            return JsonMapper.ToObject<ModuleABConfig>(request.downloadHandler.text);
        }

        return null;
    }

    /// <summary>
    /// 通过模块的AB资源json配置文件 创建内存中的资源容器，并且这个函数还返回了这个模块对应的容器！
    /// base2Assets 这个成员变量，存放了所有模块的容器对象
    /// 这个字典的键就是模块的名字
    /// 值就是代表这个模块对应的一个容器对象，存放了这个模块的所有资源
    /// </summary>
    /// <param name="moduleABConfig"></param>
    /// <returns></returns>
    public Hashtable ConfigAssembly(ModuleABConfig moduleABConfig)
    {
        Hashtable path2AssetRef = new Hashtable();
        for (int i = 0; i < moduleABConfig.AssetArray.Length; i++)
        {
            AssetInfo assetInfo = moduleABConfig.AssetArray[i];

            //装配一个AssetRef对象
            AssetRef assetRef = new AssetRef(assetInfo);
            assetRef.bundleRef = name2BundleRef[assetInfo.bundle_name];

            int count = assetInfo.dependencies.Count;
            assetRef.dependencies = new BundleRef[count];
            for (int index = 0; index < count; index++)
            {
                string bundleName = assetInfo.dependencies[index];
                assetRef.dependencies[index] = name2BundleRef[bundleName];
            }

            //装配好了放到path2AssetRef容器中
            path2AssetRef.Add(assetInfo.asset_path, assetRef);
        }

        return path2AssetRef;
    }

    /// <summary>
    /// 克隆一个GameObject对象
    /// </summary>
    /// <param name="moduelName">模块的名字</param>
    /// <param name="path"></param>
    /// <returns></returns>
    public GameObject Clone(string moduelName, string path)
    {
        AssetRef assetRef = LoadAssetRef<GameObject>(moduelName, path);
        if (assetRef == null || assetRef.asset == null)
        {
            return null;
        }

        GameObject gameObject = UnityEngine.Object.Instantiate(assetRef.asset) as GameObject;
        if (assetRef.children == null)
        {
            assetRef.children = new List<GameObject>();
        }
        assetRef.children.Add(gameObject);

        return gameObject;
    }

    /// <summary>
    /// 加载AssetRef对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="moduelName">模块名字</param>
    /// <param name="assetPath">资源的相对路径</param>
    /// <returns></returns>
    private AssetRef LoadAssetRef<T>(string moduelName, string assetPath) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (GlobalConfig.BundleMode == false)
        {
            return LoadAssetRef_Editor<T>(moduelName, assetPath);
        }
        else
        {
            return LoadAssetRef_Runtime<T>(moduelName, assetPath);
        }
#else
        return LoadAssetRef_Runtime<T>(moduelName, assetPath);  
#endif
    }

    /// <summary>
    /// 在编辑器模式下加载 AssetRef对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="moduelName">模块名字</param>
    /// <param name="assetPath">资源的相对路径</param>
    /// <returns></returns>
    private AssetRef LoadAssetRef_Editor<T>(string moduelName, string assetPath) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(assetPath))
        {
            return null;
        }

        AssetRef assetRef = new AssetRef(null);
        assetRef.asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

        return assetRef;
#else
    return null;
#endif
    }


    /// <summary>
    /// 在AB包模式下加载AssetRef对象
    /// </summary>
    /// <typeparam name="T">要加载的资源类型</typeparam>
    /// <param name="moduleName">模块名字</param>
    /// <param name="assetPath">资源的相对路径</param>
    /// <returns></returns>
    private AssetRef LoadAssetRef_Runtime<T>(string moduleName, string assetPath) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(assetPath))
        {
            return null;
        }

        Hashtable module2AssetRef = GlobalConfig.HotUpdate == true ? update2Assets[moduleName] : base2Assets[moduleName];

        AssetRef assetRef = (AssetRef)module2AssetRef[assetPath];
        if (assetRef == null)
        {
            Debug.LogError("未找到资源： " + moduleName + " path" + assetPath);
            return null;
        }

        if (assetRef.asset != null)
        {
            return assetRef;
        }

        // 1.处理assetRef依赖的BundleRef列表
        foreach (BundleRef oneBundleRef in assetRef.dependencies)
        {
            if (oneBundleRef.bundle == null)
            {
                string bundlePath = BundlePath(oneBundleRef.baseOrUpdate, moduleName, oneBundleRef.bundleInfo.bundle_name);
                oneBundleRef.bundle = AssetBundle.LoadFromFile(bundlePath);
            }

            if (oneBundleRef.children == null)
            {
                oneBundleRef.children = new List<AssetRef>();
            }

            oneBundleRef.children.Add(assetRef);
        }

        // 2.处理assetRef属于的那个BundleRef对象
        BundleRef bundleRef = assetRef.bundleRef;
        if (bundleRef.bundle == null)
        {
            bundleRef.bundle = AssetBundle.LoadFromFile(BundlePath(bundleRef.baseOrUpdate, moduleName, bundleRef.bundleInfo.bundle_name));
        }

        if (bundleRef.children == null)
        {
            bundleRef.children = new List<AssetRef>();
        }

        bundleRef.children.Add(assetRef);

        // 3.从bundle中提取asset
        assetRef.asset = assetRef.bundleRef.bundle.LoadAsset<T>(assetRef.assetInfo.asset_path);
        if (typeof(T) == typeof(GameObject) && assetRef.assetInfo.asset_path.EndsWith(".prefab"))
        {
            assetRef.isGameObject = true;
        }
        else
        {
            assetRef.isGameObject = false;
        }

        return assetRef;
    }

    /// <summary>
    /// 工具函数 更具模块名字和bundle名字 返回实际资源路径
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    private string BundlePath(BaseOrUpdate baseOrUpdate, string moduleName, string bundleName)
    {
        if (baseOrUpdate == BaseOrUpdate.Update)
        {
            return Application.persistentDataPath + "/Bundles/" + moduleName + "/" + bundleName;
        }
        else
        {
            return Application.streamingAssetsPath + "/" + moduleName + "/" + bundleName;
        }
    }


    /// <summary>
    /// 创建资源对象，并将其赋值给游戏对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="moduleName"></param>
    /// <param name="assetPath"></param>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public T CreateAsset<T>(string moduleName, string assetPath, GameObject gameObject) where T : UnityEngine.Object
    {
        if (typeof(T) == typeof(GameObject) || (!string.IsNullOrEmpty(assetPath) && assetPath.EndsWith(".prefab")))
        {
            Debug.LogError("不可以下载GameObject类型，请直接使用AssetLoader.Instance.Clone接口,path:" + assetPath);
            return null;
        }

        if (gameObject == null)
        {
            Debug.LogError("CreateAsset必须传递一个gameObject其将要被挂载的GameObject对象！");
            return null;
        }

        AssetRef assetRef = LoadAssetRef<T>(moduleName, assetPath);
        if (assetRef == null || assetRef.asset == null)
        {
            return null;
        }

        if (assetRef.children == null)
        {
            assetRef.children = new List<GameObject>();
        }

        assetRef.children.Add(gameObject);

        return assetRef.asset as T;
    }


    public void Unload(Dictionary<string, Hashtable> module2Assets)
    {
        foreach (string moduleName in module2Assets.Keys)
        {
            Hashtable path2AssetRef = module2Assets[moduleName];
            if (path2AssetRef == null)
            {
                continue;
            }

            foreach (AssetRef assetRef in path2AssetRef.Values)
            {
                if (assetRef.children == null || assetRef.children.Count == 0)
                {
                    continue;
                }

                for (int i = assetRef.children.Count - 1; i >= 0; i--)
                {
                    GameObject go = assetRef.children[i];
                    if (go == null)
                    {
                        assetRef.children.RemoveAt(i);
                    }
                }

                // 如果这个资源assetRef已经没有被任何GameObject依赖，那么此AssetRef就可以卸载了
                if (assetRef.children.Count == 0)
                {
                    assetRef.asset = null;
                    Resources.UnloadUnusedAssets();

                    // 对于assetRef所属的这个bundle，解除关系
                    assetRef.bundleRef.children.Remove(assetRef);
                    if (assetRef.bundleRef.children.Count == 0)
                    {
                        assetRef.bundleRef.bundle.Unload(true);
                    }

                    //对于assetRef所依赖的那些bundle列表，解除关系
                    foreach (BundleRef bundleRef in assetRef.dependencies)
                    {
                        bundleRef.children.Remove(assetRef);
                        if (bundleRef.children.Count == 0)
                        {
                            bundleRef.bundle.Unload(true);
                        }
                    }
                }

            }

        }
    }
}
