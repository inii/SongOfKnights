
using UnityEngine;

public class Main : MonoBehaviour
{
    private async void Awake()
    {
        InitGlobal();

        ModuleConfig launchModule = new ModuleConfig()
        {
            moduleName = "Launch",
            moduleVersion = "20230101",
            moduleUrl = "http://192.168.12.179:8000/",
        };

        //ModuleManager.Instance.Load(launchModule, (success) =>
        //{
        //    // 在这个里把代码控制权交给Lua 
        //    Debug.Log("Lua start...");
        //});

        bool result = await ModuleManager.Instance.Load(launchModule);
        if (result == true)
        {
            //在这个里把代码控制权交给Lua
            Debug.Log("Lua start...");
            AssetLoader.Instance.Clone("Launch", "Assets/GAssets/Launch/Sphere.prefab");

            GameObject testObj = AssetLoader.Instance.Clone("Launch", "Assets/GAssets/Launch/TestSpriteRenderer.prefab");
            testObj.GetComponent<SpriteRenderer>().sprite =
                AssetLoader.Instance.CreateAsset<Sprite>("Launch", "Assets/GAssets/Launch/Sprite/1216.png", testObj);
        }

    }

    private void InitGlobal()
    {
        Instance = this;
        GlobalConfig.HotUpdate = true;
        GlobalConfig.BundleMode = true;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 主Mono对象
    /// </summary>
    public static Main Instance;


    private void Update()
    {
        //执行卸载策略
        AssetLoader.Instance.Unload(AssetLoader.Instance.base2Assets);
    }
}