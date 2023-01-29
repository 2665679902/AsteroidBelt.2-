using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Log.LogUtility
{
    /// <summary>
    /// Log的等级
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        /// <summary>
        /// 调试级
        /// </summary>
        Debug = 0,

        /// <summary>
        /// 一般级
        /// </summary>
        Infor = 1,

        /// <summary>
        /// 错误级
        /// </summary>
        Error = 2,

        /// <summary>
        /// 崩溃级
        /// </summary>
        Fatal = 3,

    }
}
