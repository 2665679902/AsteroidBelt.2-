using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidBelt.UI.UIEvent
{
    /// <summary>
    /// 文本更改事件
    /// </summary>
    public enum TextEvent
    {
        /// <summary>
        /// 文本不被修改
        /// </summary>
        [Tooltip("文本不会被修改")]
        None = 0,

        /// <summary>
        /// 火箭仓状态描述文本获得信息
        /// </summary>
        [Tooltip("火箭仓状态描述文本获得信息")]
        Get_HabitatConditionText = 1,

    }
}
