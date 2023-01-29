using AsteroidBelt.UI.UIEvent;
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
    /// 滑动条事件发送者
    /// </summary>
    public class SliderEventTrigger : AsUIEventTrigger
    {
        /// <summary>
        /// 触发一个滑动条事件
        /// </summary>
        [Header("触发一个滑动条事件")]
        public SliderEvent Event = SliderEvent.None;

        /// <summary>
        /// 依赖反转由编辑器赋值
        /// </summary>
        protected override Enum EventEnum => Event;
    }
}
