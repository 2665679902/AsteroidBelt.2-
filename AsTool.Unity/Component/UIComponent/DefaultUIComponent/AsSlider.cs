using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using AsTool.Unity.Component.EventComponent;

namespace AsTool.Unity.Component.UIComponent.DefaultUIComponent
{
    /// <summary>
    /// 一个管理<see cref="UnityEngine.UI.Slider"/>的组件
    /// </summary>
    public class AsSlider: AsUIComponent
    {
        /// <summary>
        /// 内部管理的组件
        /// </summary>
        public Slider Slider { get => GetComponent<Slider>(); }

        /// <summary>
        /// 填充已选中区域的UI对象
        /// </summary>
        public RectTransform Fill { get => Slider.fillRect; set => Slider.fillRect = value; }

        /// <summary>
        /// 用于显示用于拖动的"柄"的UI对象
        /// </summary>
        public RectTransform Handle { get => Slider.handleRect; set => Slider.handleRect = value; }

        /// <summary>
        /// 滑动条的方向组件
        /// </summary>
        public Slider.Direction Direction { get => Slider.direction; set => Slider.direction = value; }

        /// <summary>
        /// 当前滑动条的最小值
        /// </summary>
        public float MinValue { get => Slider.minValue; set => Slider.minValue = value; }

        /// <summary>
        /// 当前滑动条的最大值
        /// </summary>
        public float MaxValue { get => Slider.maxValue; set => Slider.maxValue = value; }

        /// <summary>
        /// 是否仅允许当前值为整数
        /// </summary>
        public bool WholeNumbers { get => Slider.wholeNumbers; set => Slider.wholeNumbers = value; }

        /// <summary>
        /// 滑动条的当前值
        /// </summary>
        public float Value { get => Slider.value; set => Slider.value = value; }

        /// <summary>
        /// 初始化订阅值改变事件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            Slider.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 当值改变时触发此函数, 如果存在事件发送脚本, 默认传入本脚本作为<see cref="AsComponentEventArg.Sender"/>
        /// </summary>
        /// <param name="value">当前的滑块指示的值</param>
        public virtual void OnValueChanged(float value)
        {
            EventTrigger?.Trigger(this);
        }
    }
}
