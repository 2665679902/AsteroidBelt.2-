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
    /// 一个管理<see cref="UnityEngine.UI.ScrollRect"/>的组件
    /// </summary>
    public class AsScrollRect: AsUIComponent
    {
        /// <summary>
        /// 内部管理的组件
        /// </summary>
        public ScrollRect ScrollRect { get => GetComponent<ScrollRect>(); }

        /// <summary>
        /// 滚动视图内部的内容
        /// </summary>
        public RectTransform Content { get => ScrollRect.content; set => ScrollRect.content = value; }

        /// <summary>
        /// 启用水平滚动
        /// </summary>
        public bool IsHorizontal { get => ScrollRect.horizontal; set => ScrollRect.horizontal = value; }

        /// <summary>
        /// 启用竖直滚动
        /// </summary>
        public bool IsVertical { get => ScrollRect.vertical; set => ScrollRect.vertical = value; }

        /// <summary>
        /// 滚动视图的移动效果
        /// </summary>
        public ScrollRect.MovementType MovementType { get => ScrollRect.movementType; set => ScrollRect.movementType = value; }

        /// <summary>
        /// 回弹速度 (很快)[0,无穷](很慢)
        /// </summary>
        public float Elasticity { get => ScrollRect.elasticity; set => ScrollRect.elasticity = value; }

        /// <summary>
        /// 是否启用移动惯性
        /// </summary>
        public bool Intertia { get => ScrollRect.inertia; set => ScrollRect.inertia = value; }

        /// <summary>
        /// 移动惯性(很小)[0,1](很大)
        /// </summary>
        public float DecelerationRate { get => ScrollRect.decelerationRate; set => ScrollRect.decelerationRate = value; }

        /// <summary>
        /// 用于显示可视范围的对象
        /// </summary>
        public RectTransform Viewport { get => ScrollRect.viewport; set => ScrollRect.viewport = value; }

        /// <summary>
        /// 水平滚动条
        /// </summary>
        public Scrollbar HorizontalScrollbar { get => ScrollRect.horizontalScrollbar; set => ScrollRect.horizontalScrollbar = value; }

        /// <summary>
        /// 垂直滚动条
        /// </summary>
        public Scrollbar VerticalScrollbar { get => ScrollRect.verticalScrollbar; set => ScrollRect.verticalScrollbar = value; }

        /// <summary>
        /// 水平滚动条的显示模式
        /// </summary>
        public ScrollRect.ScrollbarVisibility HorizontalScrollbarVisibility { get => ScrollRect.horizontalScrollbarVisibility; set => ScrollRect.horizontalScrollbarVisibility = value; }

        /// <summary>
        /// 竖直滚动条的显示模式
        /// </summary>
        public ScrollRect.ScrollbarVisibility VerticalScrollbarVisibility { get => ScrollRect.verticalScrollbarVisibility; set => ScrollRect.verticalScrollbarVisibility = value; }

        /// <summary>
        /// 内容当前位置[0,1](百分比位置)
        /// </summary>
        public Vector2 NormalizedPosition { get => ScrollRect.normalizedPosition; set => ScrollRect.normalizedPosition = value; }

        /// <summary>
        /// 初始化订阅值改变事件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            ScrollRect.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 当值改变时触发此函数, 如果存在事件发送脚本, 默认传入本脚本作为<see cref="AsComponentEventArg.Sender"/>
        /// </summary>
        /// <param name="value">当前内容位置</param>
        public virtual void OnValueChanged(Vector2 value)
        {
            EventTrigger?.Trigger(this);
        }
    }
}
