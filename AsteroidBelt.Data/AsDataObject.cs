using AsteroidBelt.Data.String;
using AsteroidBelt.Data.templates.Building.Rocket.Habitat;
using AsTool.Assert;
using AsTool.Common.Extension;
using AsTool.IO;
using AsTool.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.Data
{
    /// <summary>
    /// 所有数据模型的基类
    /// </summary>
    public class AsDataObject
    {
        #region 序列化方案

        /// <summary>
        /// 所有此类文件的根目录
        /// </summary>
        public const string RootDirectory = "AsData";

        /// <summary>
        /// 所有类型数据实例指向唯一地址(派生类会有不同的地址), 
        /// </summary>
        public string Unique_ID { get => $"{CodeStringConfig.TitleString.AsDataObjectTitle}{this.GetType().FullName.Replace(".","_")}"; } 

        /// <summary>
        /// 向唯一文件序列化本数据
        /// </summary>
        public void Serialize()
        {
            var doc = AsType.AsSerialize(this).OwnerDocument;

            doc.Save(GetFullPath(Unique_ID));
        }

        /// <summary>
        /// 通过文件刷新目标实例的状态
        /// </summary>
        /// <param name="dataNeedRefresh">要刷新的目标</param>
        /// <returns>刷新结果</returns>
        public static AsDataObject RefreshFromFile(AsDataObject dataNeedRefresh)
        {
            AsAssert.NotNull(dataNeedRefresh, "dataNeedRefresh can not be null");

            return Deserialize(dataNeedRefresh.Unique_ID, dataNeedRefresh.GetType());
        }

        /// <summary>
        /// 通过文件刷新目标实例的状态
        /// </summary>
        /// <typeparam name="T">目标类型必须为<see cref="AsDataObject"/>的子类</typeparam>
        /// <param name="dataNeedRefresh">要刷新的目标</param>
        /// <returns>刷新结果</returns>
        public static T RefreshFromFile<T>(T dataNeedRefresh) where T : AsDataObject
        {
            return RefreshFromFile((AsDataObject)dataNeedRefresh) as T;
        }

        /// <summary>
        /// 通过文件默认实例名刷新目标实例的状态
        /// </summary>
        /// <typeparam name="T">目标类型必须为<see cref="AsDataObject"/>的子类</typeparam>
        /// <returns>刷新结果</returns>
        public static T RefreshFromFile<T>() where T : AsDataObject
        {
            return RefreshFromFile((T)AsType.GetAsType(typeof(T)).CompelInit());
        }

        /// <summary>
        /// 获取本地的完全路径名
        /// </summary>
        /// <param name="serializeFileName">文件名</param>
        /// <returns>绝对路径</returns>
        public static string GetFullPath(string serializeFileName) => AsIOConfig.GetLoaclFullPath(RootDirectory, $"AsData_{serializeFileName}.xml");

        /// <summary>
        /// 反序列化一个AsDataObject衍生实例
        /// </summary>
        /// <param name="serializeFileName">文件名</param>
        /// <param name="type">要反序列化为的类型, 必须为<see cref="AsDataObject"/>的子类, 否则会报错</param>
        /// <returns>反序列化结果, 反序列化失败会返回null</returns>
        public static AsDataObject Deserialize(string serializeFileName, Type type)
        {
            AsAssert.IsTrue(typeof(AsDataObject).IsAssignableFrom(type), "type must be child of AsDataObject");

            var doc = new System.Xml.XmlDocument();

            var path = GetFullPath(serializeFileName);

            //先尝试看一下是不是空文件
            if(string.IsNullOrWhiteSpace(AsFileManager.Local.ReadFile(path, true)))
                return null;

            doc.Load(path);

            return AsType.AsDeserialize(doc["AsData"], type) as AsDataObject;
        }

        /// <summary>
        /// 反序列化一个AsDataObject衍生实例
        /// </summary>
        /// <typeparam name="T">要反序列化为的类型, 必须为<see cref="AsDataObject"/>的子类</typeparam>
        /// <param name="serializeFileName">文件名</param>
        /// <returns>反序列化结果, 反序列化失败会返回null</returns>
        public static T Deserialize<T>(string serializeFileName) where T : AsDataObject
        {
            return (T)Deserialize(serializeFileName, typeof(T));
        }

        #endregion
    }
}
