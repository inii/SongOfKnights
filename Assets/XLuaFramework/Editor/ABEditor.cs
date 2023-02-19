using Codice.CM.WorkspaceServer.Tree.GameUI.Checkin.Updater;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 生成AssetBundle的编辑器工具 
/// 核心功能是：扫描Assets\GAssets\下的所有一级文件夹，每一个文件夹被认为是一个模块，针对每个模块，会生成其所有的AB包文件，(针对每个模块生成其AB包对应的json配置文件)！
/// </summary>
public class ABEditor : MonoBehaviour
{
    /// <summary>
    /// 热更资源的根目录
    /// </summary>
    public static string rootPath = Application.dataPath + "/GAssets";

    /// <summary>
    /// 所有需要打包的AB包信息：一个AssetBundle文件对应一个AssetBundleBuild对象
    /// </summary>
    public static List<AssetBundleBuild> assetBundleBuildList = new List<AssetBundleBuild>();

    /// <summary>
    /// AB包文件的输出路径
    /// </summary>
    public static string abOutputPath = Application.streamingAssetsPath;

    /// <summary>
    /// 记录哪个asset资源属于哪个AB包文件
    /// </summary>
    public static Dictionary<string, string> asset2bundle = new Dictionary<string, string>();

    /// <summary>
    /// 记录每个asset资源依赖的AB包文件列表
    /// </summary>
    public static Dictionary<string, List<string>> asset2Dependencies = new Dictionary<string, List<string>>();


    /// <summary>
    /// 打包AssetBundle资源
    /// </summary>
    [MenuItem("MyEditor/BuildAssetBundle")]
    public static void BuildAssetBundle()
    {
        Debug.Log("开始 --->> 开始生成所有模块的AB包!");
        if (Directory.Exists(abOutputPath) == true)
        {
            //Directory.Delete(abOutputPath, true);
            Directory.GetFiles(abOutputPath).ToList<string>().ForEach(File.Delete);
            Directory.GetDirectories(abOutputPath).ToList<string>().ForEach((dirPath) => { Directory.Delete(dirPath, true); });
        }

        // 遍历所有模块，针对所有模块都分别打包
        DirectoryInfo rootDir = new DirectoryInfo(rootPath);
        DirectoryInfo[] infos = rootDir.GetDirectories();
        foreach (DirectoryInfo dir in infos)
        {
            string moduleName = dir.Name;
            assetBundleBuildList.Clear();
            asset2bundle.Clear();
            asset2Dependencies.Clear();

            //开始给这个模块生成AB包文件
            ScanChildDireations(dir);
            AssetDatabase.Refresh();

            string moduleOutputPath = abOutputPath + "/" + moduleName;
            if (Directory.Exists(moduleOutputPath) == true)
            {
                Directory.Delete(moduleOutputPath, true);
            }
            Directory.CreateDirectory(moduleOutputPath);

            // 压缩选项详解
            // BuildAssetBundleOptions.None：使用LZMA算法压缩，压缩的包更小，但是加载时间更长。使用之前需要整体解压。一旦被解压，这个包会使用LZ4重新压缩。使用资源的时候不需要整体解压。在下载的时候可以使用LZMA算法，一旦它被下载了之后，它会使用LZ4算法保存到本地上。
            // BuildAssetBundleOptions.UncompressedAssetBundle：不压缩，包大，加载快
            // BuildAssetBundleOptions.ChunkBasedCompression：使用LZ4压缩，压缩率没有LZMA高，但是我们可以加载指定资源而不用解压全部

            // 参数一: bundle文件列表的输出路径
            // 参数二：生成bundle文件列表所需要的AssetBundleBuild对象数组（用来指导Unity生成哪些bundle文件，每个文件的名字以及文件里包含哪些资源）
            // 参数三：压缩选项BuildAssetBundleOptions.None默认是LZMA算法压缩
            // 参数四：生成哪个平台的bundle文件，即目标平台   File->BuildSettings可查看，AB包是跟平台相关的，所以针对每个不同的平台(Win,Android,IOS)要分别打AB包!
            BuildPipeline.BuildAssetBundles(moduleOutputPath, assetBundleBuildList.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            CalculateDependencies();

            SaveModuleABConfig(moduleName);

            AssetDatabase.Refresh();
        }

        Debug.Log("结束 --->> 生成所有模块的AB包!");
    }

    /// <summary>
    /// 根据指定的文件夹，递归遍历，将这个文件夹下的所有一级子文件打成一个AssetBundle
    /// </summary>
    /// <param name="dirInfo"></param>
    public static void ScanChildDireations(DirectoryInfo dirInfo)
    {
        if (dirInfo.Name.EndsWith("CSProject~"))
        {
            return;
        }

        // 收集当前路径下的文件 把它们打成一个AB包
        ScanCurrentDirectory(dirInfo);

        //遍历子文件夹
        foreach (DirectoryInfo info in dirInfo.GetDirectories())
        {
            ScanChildDireations(info);
        }
    }

    public static void ScanCurrentDirectory(DirectoryInfo dirInfo)
    {
        List<string> assetNames = new List<string>();
        FileInfo[] fileInfoList = dirInfo.GetFiles();
        foreach (FileInfo fileInfo in fileInfoList)
        {
            if (fileInfo.FullName.EndsWith(".meta"))
            {
                continue;
            }

            // assetName的格式类似 "Assets/GAssets/Launch/Sphere.prefab"
            string assetName = fileInfo.FullName.Substring(Application.dataPath.Length - "Assets".Length).Replace("\\", "/");
            assetNames.Add(assetName);
        }

        if (assetNames.Count > 0)
        {
            // 格式类似 gassets_launch
            string assetbundleName = dirInfo.FullName.Substring(Application.dataPath.Length + 1).Replace("\\", "_").ToLower();
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = assetbundleName;
            build.assetNames = new string[assetNames.Count];
            for (int i = 0; i < assetNames.Count; i++)
            {
                build.assetNames[i] = assetNames[i];
                //记录单个资源属于哪个bundle文件
                asset2bundle.Add(assetNames[i], assetbundleName);
            }

            assetBundleBuildList.Add(build);
        }
    }

    public static void CalculateDependencies()
    {
        foreach (string assetKey in asset2bundle.Keys)
        {
            //这个资源自己所在的bundle
            string assetBundle = asset2bundle[assetKey];
            string[] dependencies = AssetDatabase.GetDependencies(assetKey);
            List<string> assetList = new List<string>();

            if (dependencies != null && dependencies.Length > 0)
            {
                foreach (string depend in dependencies)
                {
                    if (depend == assetKey || depend.EndsWith(".cs"))
                    {
                        continue;
                    }

                    assetList.Add(depend);
                }
            }

            if (assetList.Count > 0)
            {
                List<string> abList = new List<string>();
                foreach (string asset in assetList)
                {
                    bool result = asset2bundle.TryGetValue(asset, out string bundle);
                    if (result == true)
                    {
                        if (bundle != assetBundle)
                        {
                            abList.Add(asset);
                        }
                    }
                }

                asset2Dependencies.Add(assetKey, abList);
            }

        }
    }


    /// <summary>
    /// 将一个模块的资源依赖关系数据保存成json格式的文件
    /// </summary>
    /// <param name="moduleName">模块名字</param>
    public static void SaveModuleABConfig(string moduleName)
    {
        ModuleABConfig moduleABConfig = new ModuleABConfig(asset2bundle.Count);

        //记录AB包信息 moduleName
        foreach (AssetBundleBuild abb in assetBundleBuildList)
        {
            BundleInfo bundleInfo = new BundleInfo();
            bundleInfo.bundle_name = abb.assetBundleName;
            bundleInfo.assets = new List<string>();

            foreach (string asName in abb.assetNames)
            {
                bundleInfo.assets.Add(asName);
            }

            // 计算一个bundle文件的CRC散列码
            string abFilePath = abOutputPath + "/" + moduleName + "/" + bundleInfo.bundle_name;
            using (FileStream fstream = File.OpenRead(abFilePath))
            {
                bundleInfo.crc = AssetUtility.GetCRC32Hash(fstream);
                // 计算一个bundle文件的大小
                bundleInfo.size = (int)fstream.Length;
            }

            moduleABConfig.AddBundle(bundleInfo.bundle_name, bundleInfo);
        }

        // 记录每个资源的依赖关系
        int astIndex = 0;
        foreach (var item in asset2bundle)
        {
            AssetInfo astInfo = new AssetInfo();
            astInfo.asset_path = item.Key;
            astInfo.bundle_name = item.Value;
            astInfo.dependencies = new List<string>();

            bool result = asset2Dependencies.TryGetValue(item.Key, out List<string> dependencies);
            if (result == true)
            {
                for (int i = 0; i < dependencies.Count; i++)
                {
                    string bdName = dependencies[i];
                    astInfo.dependencies.Add(bdName);
                }
            }

            moduleABConfig.AddAsset(astIndex, astInfo);
            astIndex++;
        }

        //开始写入Json文件
        string moduleConfigName = moduleName.ToLower() + ".json";
        string jsonPath = abOutputPath + "/" + moduleName + "/" + moduleConfigName;
        if (File.Exists(jsonPath) == true)
        {
            File.Delete(jsonPath);
        }

        File.Create(jsonPath).Dispose();
        string jsonData = LitJson.JsonMapper.ToJson(moduleABConfig);
        File.WriteAllText(jsonPath, ConvertJsonString(jsonData));
    }

    /// <summary>
    /// 格式化json
    /// </summary>
    /// <param name="str">输入json字符串</param>
    /// <returns>返回格式化后的字符串</returns>
    private static string ConvertJsonString(string str)
    {
        JsonSerializer serializer = new JsonSerializer();
        TextReader tr = new StringReader(str);
        JsonTextReader jtr = new JsonTextReader(tr);

        object obj = serializer.Deserialize(jtr);
        if (obj != null)
        {
            StringWriter textWriter = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };

            serializer.Serialize(jsonWriter, obj);

            return textWriter.ToString();
        }
        else
        {
            return str;
        }
    }

}
