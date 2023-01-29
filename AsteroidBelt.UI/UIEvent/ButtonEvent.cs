using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidBelt.UI.UIEvent
{
    /// <summary>
    /// 按钮点击事件
    /// </summary>
    public enum ButtonEvent
    {
        /// <summary>
        /// 没有事件
        /// </summary>
        [Tooltip("本事件触发器不会触发任何事件")]
        None = 0,

        /// <summary>
        /// 保存设计下拉框组数据
        /// </summary>
        [Tooltip("保存设计下拉框组数据")]
        HabitatDropdownGroupSave = 1,

        /// <summary>
        /// 设置默认值但不保存设计下拉框组数据
        /// </summary>
        [Tooltip("设置默认值但不保存设计下拉框组数据")]
        HabitatDropdownGroupSetDefault = 2,

        /// <summary>
        /// 关闭根面板
        /// </summary>
        [Tooltip("关闭根面板")]
        Close_ModConfig_Root = 3,
    }
}
