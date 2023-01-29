using AsteroidBelt.UI.UIEvent;
using AsTool.Unity.Component.EventComponent;
using AsTool.Unity.Component.UIComponent.UIEventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace AsteroidBelt.UI.Component.EventComponent
{
    /// <summary>
    /// 用于监听SliderEvent事件，处理方法为尝试将Data数据转化为浮点数，并尝试将其赋值给同一 GameObject 的 Slider 组件 
    /// </summary>
    public class SliderEventObserver : AsUIEventObserver
    {
        /// <summary>
        /// 接收到目标事件时会尝试改变同一 GameObject 的 Slider 文本
        /// </summary>
        [Header("接收到目标事件时会尝试改变同一 GameObject 的 Slider 文本")]
        public SliderEvent ChangeEvent = SliderEvent.None;

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
            if (gameObject.GetComponent<Slider>() is null)
            {
                AsLog.Error($"{gameObject} lost Slider component");
            }
            else
            {
                GetComponent<Slider>().value = eventArg?.Data as float? ?? GetComponent<Slider>().value;
            }

            return eventArg;
        }
    }
}
