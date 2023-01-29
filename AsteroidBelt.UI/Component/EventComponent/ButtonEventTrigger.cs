using AsteroidBelt.UI.UIEvent;
using AsTool.Extension;
using AsTool.Unity.Component.EventComponent;
using AsTool.Unity.Component.UIComponent.UIEventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidBelt.UI.Component.EventComponent
{
    /// <summary>
    /// 按钮事件发送者
    /// </summary>
    public class ButtonEventTrigger : AsUIEventTrigger
    {
        /// <summary>
        /// 触发一个按钮事件
        /// </summary>
        [Header("触发一个按钮事件")]
        public ButtonEvent Event = ButtonEvent.None;

        /// <summary>
        /// 依赖反转由编辑器赋值
        /// </summary>
        protected override Enum EventEnum => Event;
    }
}
