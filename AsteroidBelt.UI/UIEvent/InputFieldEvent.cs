using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidBelt.UI.UIEvent
{
    /// <summary>
    /// 输入框输入结束事件和接收文本事件
    /// </summary>
    public enum InputFieldEvent
    {
        /// <summary>
        /// 没有事件
        /// </summary>
        [Tooltip("本事件触发器不会触发任何事件, 或导致任何事件被监听")]
        None = 0,

        /// <summary>
        /// 火箭仓状态尺寸描述输入框获得信息
        /// </summary>
        [Tooltip("火箭仓状态尺寸描述输入框获得信息")]
        Get_HabitatSizeText = 1,

        /// <summary>
        /// 火箭仓状态尺寸描述输入框输入完毕
        /// </summary>
        [Tooltip("火箭仓状态尺寸描述输入框输入完毕")]
        InputEnd_HabitatSizeText = 2,
    }
}
