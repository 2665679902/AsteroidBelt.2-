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
    /// 一个管理<see cref="UnityEngine.UI.Scrollbar"/>的组件
    /// </summary>
    public class AsScrollbar : AsUIComponent
    {
        /// <summary>
        /// 内部管理的组件
        /// </summary>
        public Scrollbar Scrollbar { get => GetComponent<Scrollbar>(); }

        /// <summary>
        /// "柄"对象
        /// </summary>
        public RectTransform HandleRect { get => Scrollbar.handleRect; set => Scrollbar.handleRect = value; }

        /// <summary>
        /// 滑动块增加的方向
        /// </summary>
        public Scrollbar.Direction Direction { get => Scrollbar.direction; set => Scrollbar.direction = value; }

        /// <summary>
        /// 当前的值
        /// </summary>
        public float Value { get => Scrollbar.value; set => Scrollbar.value = value; }

        /// <summary>
        /// 当前滑动块在调中所占的比例
        /// </summary>
        public float Size { get => Scrollbar.size; set => Scrollbar.size = value; }

        /// <summary>
        /// 当前滑动块的步数, 默认为0 则不设步数
        /// </summary>
        public int NumberOfSteps { get => Scrollbar.numberOfSteps; set => Scrollbar.numberOfSteps = value; }

        /// <summary>
        /// 初始化订阅值改变事件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            Scrollbar.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 当值改变时触发此函数, 如果存在事件发送脚本, 默认传入本脚本作为<see cref="AsComponentEventArg.Sender"/>
        /// </summary>
        /// <param name="value">当前的滑块值</param>
        public virtual void OnValueChanged(float value)
        {
            EventTrigger?.Trigger(this);
        }
    }
}
