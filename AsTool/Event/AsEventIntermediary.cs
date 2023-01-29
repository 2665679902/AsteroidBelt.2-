using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Event
{
    /// <summary>
    /// 事件中介者, 会将一个事件推送到另一个事件
    /// </summary>
    public abstract class AsEventIntermediary: AsEventReceiver
    {
        /// <summary>
        /// 订阅的事件
        /// </summary>
        public abstract string SubscribeId { get; }

        /// <summary>
        /// 要转发的事件
        /// </summary>
        public abstract string TriggerId { get; }

        /// <summary>
        /// 私有的要触发的目标事件
        /// </summary>
        private AsEventSubject subject;

        /// <summary>
        /// 初始化时, 构建要触发的目标事件
        /// </summary>
        public AsEventIntermediary()
        {
            subject = AsEvent.GetAsSubject(TriggerId);
        }

        /// <summary>
        /// 默认直接触发目标事件, 不对消息进行处理
        /// </summary>
        /// <param name="data">传入消息</param>
        /// <returns>传出后修改的结果</returns>
        public override object Action(object data)
        {
            return subject.Trigger(data, ParameterType);
        }
    }
}
