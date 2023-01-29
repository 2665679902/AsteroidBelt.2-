using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Extension
{
    /// <summary>
    /// 枚举类型拓展方法
    /// </summary>
    public static class AsEnumExtension
    {
        /// <summary>
        /// 将枚举序列化为唯一的字符串
        /// </summary>
        /// <param name="enum">枚举项</param>
        /// <returns>唯一指向的字符串</returns>
        public static string AsToString(this Enum @enum)
        {
            return $"{@enum.GetType()}_{@enum}";
        }
    }
}
