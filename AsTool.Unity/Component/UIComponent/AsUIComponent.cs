using AsTool.Event;
using AsTool.Unity.Component.EventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Unity.Component.UIComponent
{
    /// <summary>
    /// 所有UI组件管理者的基类
    /// </summary>
    public class AsUIComponent: AsMonoBehaviour
    {
        /// <summary>
        /// 默认在本游戏对象中获取事件触发脚本
        /// </summary>
        public virtual AsComponentEventTrigger EventTrigger { get; set; }

        /// <summary>
        /// 初始化时查找事件触发脚本
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            EventTrigger = GetComponent<AsComponentEventTrigger>();
        }
    }
}
