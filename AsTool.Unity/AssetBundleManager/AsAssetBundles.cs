using AsTool.Assert;
using AsTool.IO;
using AsTool.Load;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace AsTool.Unity.AssetBundleManager
{
    /// <summary>
    /// 一个管理所有asset资源的工具类
    /// </summary>
    public static class AsAssetBundles
    {
        /// <summary>
        /// 热加载包所处的文件夹名称
        /// </summary>
        public const string AssetBundleDirectory = @"assetbundles";

        /// <summary>
        /// 管理加载的Ab包
        /// </summary>
        private static Dictionary<string, AssetBundle> AssetBundles { get; set; } = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 管理加载的对象
        /// </summary>
        private static Dictionary<string, List<UnityEngine.Object>> GameObjects { get; set; } = new Dictionary<string, List<UnityEngine.Object>>();

        [AsLoad(9)]
        internal static void Load()
        {
            var local = AsIOConfig.GetLoaclFullPath(AssetBundleDirectory);

#if DEBUG
            AsFileManager.Local.FileDestory(AssetBundleDirectory, fileName: false);

            AsFileManager.Local.Touch(AssetBundleDirectory, fileName: false);

            var soursePaths = new List<string>()
            {
                //"E:\\DataForIDE\\2022_ONI\\ONIDebuger\\1\\As_Debug_0\\ServerData\\StandaloneWindows64",
                "E:\\DataForIDE\\2022_ONI\\AsteroidBelt\\3\\As_Main_3\\Library\\com.unity.addressables\\aa\\Windows\\StandaloneWindows64"
            };

            foreach (var path in soursePaths)
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    File.Copy(file, Path.Combine(local,$"AsAsset_{Path.GetFileName(file)}"));
                }
            }

            AsLog.Info("自动刷新资产 Atuo Refesh AssetBundle");
#endif

            AsFileManager.Local.Touch(AssetBundleDirectory, fileName: false);

            foreach (var file in Directory.GetFiles(local).Where(s => s.EndsWith(".bundle")))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var ab = AssetBundle.LoadFromFile(file);

                AsAssert.NotNull(ab, $"load assetbundle {file} failed");

                GameObjects.Add(name, ab.LoadAllAssets()?.ToList());
                AssetBundles.Add(name, ab);
            }

            AsLog.Info($"本地资源加载完成 AsAssetBundles load finished, FileList: " + (AssetBundles.Any() ? AssetBundles.Keys.Aggregate((a, b) => a + ", " + b + ". ") : "No AssetBundle"));

        }

        /// <summary>
        /// 尝试从某个加载的Ab包中取出类
        /// </summary>
        /// <typeparam name="T">取出的对象的类型</typeparam>
        /// <param name="name">对象的名字</param>
        /// <param name="assetBundleName">Ab包的去除后缀的文件名，不填默认在所有已加载的文件中查找</param>
        /// <returns>返回找到的实例，没有找到就返回 null</returns>
        public static T GetObject<T>(string name, string assetBundleName = null) where T : UnityEngine.Object
        {

            bool Test(UnityEngine.Object obj)
            {
                if (obj.name == name)
                {
                    if (obj is T)
                    {
                        return true;
                    }
                    else
                    {
                        AsLog.Info($"意外事件: 获取了同名的其他类型组件 AsAssetBundles get [{name}], but in a wrong Type [{obj.GetType().Name}]");
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }

            if (assetBundleName == null)
            {
                foreach (var objList in GameObjects)
                {
                    foreach (var obj in objList.Value)
                    {
                        if (Test(obj))
                        {
                            return (T)obj;
                        }

                    }
                }
            }
            else
            {
                if (GameObjects.TryGetValue(assetBundleName, out List<UnityEngine.Object> resList))
                {
                    foreach (UnityEngine.Object obj in resList)
                    {
                        if (Test(obj))
                        {
                            return (T)obj;
                        }
                    }
                }
                else
                {
                    AsLog.Error($"Cam't Find AssetBundle {assetBundleName}!");

                    return null;
                }

            }


            AsLog.Error($"Get obj failed! When try to get <{typeof(T).Name}> {name}" + (assetBundleName == null ? null : $" in AssetBundle {assetBundleName}"));

            return null;
        }

        /// <summary>
        /// 尝试从某个加载的Ab包中取出GameObject并生成
        /// </summary>
        /// <param name="name">对象的名字</param>
        /// <param name="assetBundleName">Ab包的去除后缀的文件名，不填默认在所有已加载的文件中查找</param>
        /// <returns>返回找到的实例，没有找到就返回 null</returns>
        public static GameObject GetInstantiatedGameObject(string name, string assetBundleName = null)
        {
            GameObject obj = GetObject<GameObject>(name, assetBundleName);

            if (obj == null)
            {
                return null;
            }

            return UnityEngine.Object.Instantiate(obj);
        }
    }
}
