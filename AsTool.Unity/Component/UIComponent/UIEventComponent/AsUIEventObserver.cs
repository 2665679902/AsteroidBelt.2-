using AsTool.Extension;
using AsTool.Unity.Component.EventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Unity.Component.UIComponent.UIEventComponent
{
    /// <summary>
    /// 一个UI事件接收者, 会接收目标事件
    /// </summary>
    public abstract class AsUIEventObserver: AsComponentEventObserver
    {
        /// <summary>
        /// 此数据在派生类中默认由枚举类转化而来, 尽量不要重写
        /// </summary>
        protected override string EventId => EventEnum.AsToString();

        /// <summary>
        /// 此枚举类需要在子类中改写为要触发的枚举对象
        /// </summary>
        protected abstract Enum EventEnum { get; }
    }
}
