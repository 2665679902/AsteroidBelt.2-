using AsTool.Unity.Component.EventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine.UI;

namespace AsTool.Unity.Component.UIComponent.DefaultUIComponent
{
    /// <summary>
    /// 管理按钮组件的脚本
    /// </summary>
    public class AsButton: AsUIComponent
    {
        /// <summary>
        /// 内部管理的组件
        /// </summary>
        public Button Button { get => GetComponent<Button>(); }

        /// <summary>
        /// 按钮的背景图片, 没有则为null
        /// </summary>
        public Image InnerImage { get => Button.targetGraphic as Image; set => Button.targetGraphic = value; }

        /// <summary>
        /// 按钮的内部的第一个找到的文本, 没有则为null
        /// </summary>
        public Text InnerText { get; protected set; }

        /// <summary>
        /// 当前组件是否是可交互的
        /// </summary>
        public bool InterActable { get => Button.interactable; set => Button.interactable = value; }

        /// <summary>
        /// 过度效果
        /// </summary>
        public Button.Transition Transition { get => Button.transition; set => Button.transition = value; }

        /// <summary>
        /// 组件的颜色过度效果
        /// </summary>
        public ColorBlock ColorBlock { get => Button.colors; set => Button.colors = value; }

        /// <summary>
        /// 组件的图片过度效果
        /// </summary>
        public SpriteState SpriteState { get => Button.spriteState; set => Button.spriteState = value; }

        /// <summary>
        /// 本脚本的导航模式
        /// </summary>
        public Navigation Navigation { get => Button.navigation; set => Button.navigation = value; }

        /// <summary>
        /// 在初始化时绑定按键触发事件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            Button.onClick.AddListener(OnClick);
        }

        /// <summary>
        /// 在按钮点击时触发此函数, 如果存在事件发送脚本, 默认传入本脚本作为<see cref="AsComponentEventArg.Sender"/>
        /// </summary>
        public virtual void OnClick()
        {
            EventTrigger?.Trigger(this);
        }
    }
}
