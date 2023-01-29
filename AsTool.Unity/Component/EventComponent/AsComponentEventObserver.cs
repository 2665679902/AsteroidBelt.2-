using AsTool.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;

namespace AsTool.Unity.Component.EventComponent
{
    /// <summary>
    /// 一个事件接收者, 会接收目标事件
    /// </summary>
    public abstract class AsComponentEventObserver: AsMonoBehaviour
    {
        /// <summary>
        /// 要接收的事件id， 在派生类中重写
        /// </summary>
        protected abstract string EventId { get; }

        /// <summary>
        /// 内部持有的事件接收者
        /// </summary>
        protected AsEventObserver eventObserver;

        /// <summary>
        /// 在初始化时注册事件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            eventObserver = AsEvent.Subscribe(EventId);

            eventObserver.SetBehavior<AsComponentEventArg>(GetEvent);
        }

        /// <summary>
        /// 销毁时注销事件订阅者
        /// </summary>
        protected override void OnDestroy()
        {
            eventObserver.AbandonObserver();

            base.OnDestroy();
        }

        /// <summary>
        /// 在获取事件时激活此委托
        /// </summary>
        public Func<AsComponentEventArg, AsComponentEventArg> OnGetEvent { get; set; }

        /// <summary>
        /// 获取事件的函数
        /// </summary>
        /// <param name="eventArg">事件信息</param>
        /// <returns>修改后的事件信息</returns>
        private AsComponentEventArg GetEvent(AsComponentEventArg eventArg)
        {
            if(OnGetEvent is null)
                return DealEvent(eventArg);

            return OnGetEvent(DealEvent(eventArg));
        }

        /// <summary>
        /// 处理事件的函数, 先于<see cref="OnGetEvent"/>触发
        /// </summary>
        /// <param name="eventArg">事件信息</param>
        /// <returns>修改后的事件信息</returns>
        protected abstract AsComponentEventArg DealEvent(AsComponentEventArg eventArg);
    }
}
