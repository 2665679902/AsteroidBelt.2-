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
    /// 一个管理<see cref="UnityEngine.UI.Dropdown"/>的组件
    /// </summary>
    public class AsDropdown : AsUIComponent
    {
        /// <summary>
        /// 内部管理的组件
        /// </summary>
        public Dropdown Dropdown { get => GetComponent<Dropdown>(); }

        /// <summary>
        /// 当前下拉框的背景图
        /// </summary>
        public Image BackGround { get => Dropdown.targetGraphic as Image; set => Dropdown.targetGraphic = value; }

        /// <summary>
        /// 最终显示用的文字组件
        /// </summary>
        public Text CaptionText { get => Dropdown.captionText; set => Dropdown.captionText = value; }

        /// <summary>
        /// 最终显示用的图片组件
        /// </summary>
        public Image CaptionImage { get => Dropdown.captionImage; set => Dropdown.captionImage = value; }

        /// <summary>
        /// 当前组件是否是可交互的
        /// </summary>
        public bool InterActable { get => Dropdown.interactable; set => Dropdown.interactable = value; }

        /// <summary>
        /// 关联的下拉对象(一般是一个ScrollRect)
        /// </summary>
        public RectTransform Template { get => Dropdown.template; set => Dropdown.template = value; }

        /// <summary>
        /// 当前所选的索引选项的索引值
        /// </summary>
        public int Value { get => Dropdown.value; set => Dropdown.value = value; }

        /// <summary>
        /// 淡入淡出的效果的速度 (快)[0,1](慢)
        /// </summary>
        public float AlphaFadeSpeed { get => Dropdown.alphaFadeSpeed; set => Dropdown.alphaFadeSpeed = value; }

        /// <summary>
        /// 下拉框内部的选项
        /// </summary>
        public List<Dropdown.OptionData> Options { get => Dropdown.options; set => Dropdown.options = value; }

        /// <summary>
        /// 初始化订阅值改变事件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            Dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 当值改变时触发此函数, 如果存在事件发送脚本, 默认传入本脚本作为<see cref="AsComponentEventArg.Sender"/>
        /// </summary>
        /// <param name="value">当前的下拉列表的索引值</param>
        public virtual void OnValueChanged(int value)
        {
            EventTrigger?.Trigger(this);

            ValueChanging?.Invoke(value);
        }

        /// <summary>
        /// 当下拉框数值改变时触发此事件
        /// </summary>
        public event Action<int> ValueChanging;
    }
}
