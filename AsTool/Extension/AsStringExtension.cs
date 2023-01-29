using AsTool.Assert;
using AsTool.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Common.Extension
{
    /// <summary>
    /// 字符串的拓展方法
    /// </summary>
    public static class AsStringExtension
    {
        /// <summary>
        /// 尝试移除所有相关字符串，并返回新字符串
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <param name="target">要移除的字符串</param>
        /// <returns>移除结果</returns>
        public static string AsTryRemove(this string str, params string[] target)
        {
            AsAssert.NotNull(str, "AsStringExtension TryRemove: str");
            AsAssert.NotNull(target, "AsStringExtension TryRemove: target");

            var result = str;

            foreach (var item in target)
                result = result.Replace(item, string.Empty);

            return result;
        }
    }
}
