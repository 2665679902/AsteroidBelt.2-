using AsTool.Load;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Common
{
    /// <summary>
    /// 程序状态
    /// </summary>
    public static class AsApplicationCondition
    {
        /// <summary>
        /// 加载必要的检查
        /// </summary>
        [AsLoad]
        internal static void Load()
        {
            Application.quitting += () => IsQuitting = true;
        }

        /// <summary>
        /// 程序是否正在退出
        /// </summary>
        public static bool IsQuitting { get; private set; } = false;
    }
}
