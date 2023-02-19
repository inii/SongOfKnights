/// <summary>
/// 全局配置
/// </summary>
public static class GlobalConfig
{
    /// <summary>
    /// 是否开启热更
    /// </summary>
    public static bool HotUpdate;

    /// <summary>
    /// 是否采用bundle方式加载
    /// </summary>
    public static bool BundleMode;

    /// <summary>
    /// 全局配置的构造函数
    /// </summary>
    static GlobalConfig()
    {
        HotUpdate = false;

        BundleMode = false;
    }
}


/// <summary>
/// 单个模块的配置对象
/// </summary>
public class ModuleConfig
{
    /// <summary>
    /// 模块资源在远程服务器上的基础地址
    /// </summary>
    public string DownloadURL
    {
        get { return moduleUrl + "/" + moduleName + "/" + moduleVersion; }
    }

    /// <summary>
    /// 模块的名字
    /// </summary>
    public string moduleName;

    /// <summary>
    /// 模块的版本号
    /// </summary>
    public string moduleVersion;

    /// <summary>
    /// 模块的热更服务器的地址
    /// </summary>
    public string moduleUrl;
}

/// <summary>
/// 选择原始只读路径还是可读写路径
/// </summary>
public enum BaseOrUpdate
{
    /// <summary>
    /// APP安装时，生成的原始制度路径
    /// </summary>
    Base,

    /// <summary>
    /// APP提供的 可读写路径
    /// </summary>
    Update,
}