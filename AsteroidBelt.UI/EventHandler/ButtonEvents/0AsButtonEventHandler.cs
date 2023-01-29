using AsteroidBelt.UI.UIEvent;
using AsTool.Event;
using AsTool.Extension;
using AsTool.Unity.Component.EventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.UI.EventHandler.ButtonEvents
{
    /// <summary>
    /// 按钮事件处理者, 处理<see cref="ButtonEvent"/>事件
    /// </summary>
    public abstract class AsButtonEventHandler : AsEventReceiver<AsComponentEventArg>
    {
        /// <summary>
        /// 要处理的事件
        /// </summary>
        protected abstract ButtonEvent Event { get; }

        /// <summary>
        /// Id 默认为<see cref="ButtonEvent"/>转换而来, 避免重写此属性
        /// </summary>
        public override string Id => Event.AsToString();
    }
}
