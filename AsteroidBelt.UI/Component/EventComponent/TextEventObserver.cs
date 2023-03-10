using AsteroidBelt.UI.UIEvent;
using AsTool.Unity.Component.EventComponent;
using AsTool.Unity.Component.UIComponent.UIEventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AsteroidBelt.UI.Component.EventComponent
{
    /// <summary>
    /// 用于监听Text事件，处理方法为<see cref="object.ToString"/>Data数据，并尝试将其赋值给同一 GameObject 的 Text 组件
    /// </summary>
    public class TextEventObserver : AsUIEventObserver
    {
        /// <summary>
        /// 接收到目标事件时会尝试改变同一 GameObject 的 Text 文本
        /// </summary>
        [Header("接收到目标事件时会尝试改变同一 GameObject 的 Text 文本")]
        public TextEvent ChangeEvent = TextEvent.None;

        /// <summary>
        /// 依赖反转由编辑器赋值
        /// </summary>
        protected override Enum EventEnum => ChangeEvent;

        /// <summary>
        /// 处理事件函数
        /// </summary>
        /// <param name="eventArg">传入数据</param>
        /// <returns>不更改</returns>
        protected override AsComponentEventArg DealEvent(AsComponentEventArg eventArg)
        {
            if (gameObject.GetComponent<Text>() is null)
            {
                AsLog.Error($"{gameObject} lost text component");
            }
            else
            {
                GetComponent<Text>().text = eventArg?.Data?.ToString() ?? $"Null data from {eventArg.Sender}";
            }


            return eventArg;
        }
    }
}
