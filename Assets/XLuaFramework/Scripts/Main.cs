
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        InitGlobal();

        ModuleConfig launchModule = new ModuleConfig()
        {
            moduleName = "Launch",
            moduleVersion = "20230101",
            moduleUrl = "http://192.168.0.7:8000",
        };

        ModuleManager.Instance.Load(launchModule, (success) =>
        {
            // 在这个里把代码控制权交给Lua 
            Debug.Log("Lua start...");
        });
    }

    private void InitGlobal()
    {
        Instance = this;
        GlobalConfig.HotUpdate= false;
        GlobalConfig.BundleMode= false;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 主Mono对象
    /// </summary>
    public static Main Instance;

}