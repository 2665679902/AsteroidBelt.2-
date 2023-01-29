using AsTool.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Component.EventComponent
{
    /// <summary>
    /// 事件发送者
    /// </summary>
    public abstract class AsComponentEventTrigger: AsMonoBehaviour
    {

        /// <summary>
        /// 事件信息, 默认返回空, 需要重写
        /// </summary>
        protected abstract string EventId { get; }

        /// <summary>
        /// 触发一个事件， 不传入数据
        /// </summary>
        public void Trigger(UnityEngine.Component sender)
        {
            _ = AsEvent.Trigger(EventId, new AsComponentEventArg() { Data = null, Sender = sender});
        }

        /// <summary>
        /// 触发一个事件
        /// </summary>
        /// <param name="sender">发送信息的脚本</param>
        /// <param name="data">传入数据</param>
        public void Trigger(UnityEngine.Component sender, object data)
        {
            _ = AsEvent.Trigger(EventId, new AsComponentEventArg() { Data = data, Sender = sender });
        }
    }
}
