using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.Data
{
    /// <summary>
    /// 一些专用的拓展方法
    /// </summary>
    public static class AsDataObjectExtension
    {
        /// <summary>
        /// 通过文件刷新目标实例的状态
        /// </summary>
        /// <typeparam name="T">目标类型必须为<see cref="AsDataObject"/>的子类</typeparam>
        /// <param name="dataNeedRefresh">要刷新的目标</param>
        /// <returns>刷新结果</returns>
        public static T RefreshFromFile<T>(this T dataNeedRefresh) where T: AsDataObject
        {
            return AsDataObject.RefreshFromFile(dataNeedRefresh);
        }
    }
}
