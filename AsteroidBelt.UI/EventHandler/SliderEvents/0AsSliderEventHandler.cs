using AsteroidBelt.UI.UIEvent;
using AsTool.Event;
using AsTool.Extension;
using AsTool.Unity.Component.EventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.UI.EventHandler.SliderEvents
{
    /// <summary>
    /// 滑动条事件处理者, 处理<see cref="SliderEvent"/>事件
    /// </summary>
    public abstract class AsSliderEventHandler : AsEventReceiver<AsComponentEventArg>
    {
        /// <summary>
        /// 要处理的事件
        /// </summary>
        protected abstract SliderEvent Event { get; }

        /// <summary>
        /// Id 默认为<see cref="SliderEvent"/>转换而来, 避免重写此属性
        /// </summary>
        public override string Id => Event.AsToString();
    }
}
