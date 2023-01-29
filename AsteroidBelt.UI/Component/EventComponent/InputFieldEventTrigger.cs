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
    /// 输入框输入结束事件发送者
    /// </summary>
    public class InputFieldEventTrigger : AsUIEventTrigger
    {
        /// <summary>
        /// 触发一个输入框输入结束事件
        /// </summary>
        [Header("触发一个输入框输入结束事件")]
        public InputFieldEvent Event = InputFieldEvent.None;

        /// <summary>
        /// 依赖反转由编辑器赋值
        /// </summary>
        protected override Enum EventEnum => Event;
    }
}
