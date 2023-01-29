using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidBelt.UI.UIEvent
{
    /// <summary>
    /// 滑动条数值改变事件
    /// </summary>
    public enum SliderEvent
    {
        /// <summary>
        /// 没有事件
        /// </summary>
        [Tooltip("本事件触发器不会触发任何事件")]
        None = 0,

        /// <summary>
        /// 使设计的太空舱尺寸发生变化
        /// </summary>
        [Tooltip("使设计的太空舱尺寸发生变化")]
        HabitatDesignSizeChange,

        /// <summary>
        /// 设计的太空舱尺寸发生了变化
        /// </summary>
        [Tooltip("设计的太空舱尺寸发生了变化")]
        Get_HabitatDesignSizeChange,
    }
}
