using AsTool.Unity.Component.EventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace AsTool.Unity.Component.UIComponent.DefaultUIComponent
{
    /// <summary>
    /// 一个用于管理<see cref="UnityEngine.UI.Toggle"/>的组件
    /// </summary>
    public class AsToggle: AsUIComponent
    {
        /// <summary>
        /// 内部管理的组件
        /// </summary>
        public Toggle Toggle { get => GetComponent<Toggle>(); }

        /// <summary>
        /// 返回本开关是否开启
        /// </summary>
        public bool IsOn { get => Toggle.isOn; set => Toggle.isOn = value; }

        /// <summary>
        /// 返回本开关是否可以交互
        /// </summary>
        public bool Interactable { get => Toggle.interactable; set => Toggle.interactable = value; }

        /// <summary>
        /// 初始化订阅值改变事件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            Toggle.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 当值改变时触发此函数, 如果存在事件发送脚本, 默认传入本脚本作为<see cref="AsComponentEventArg.Sender"/>
        /// </summary>
        /// <param name="value"><see cref="Toggle.isOn"/></param>
        public virtual void OnValueChanged(bool value)
        {
            EventTrigger?.Trigger(this);
        }
    }
}
