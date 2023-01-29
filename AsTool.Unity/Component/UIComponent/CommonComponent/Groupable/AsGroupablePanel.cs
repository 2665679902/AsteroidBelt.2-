using AsTool.Unity.Component.UIComponent.DefaultUIComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Unity.Component.UIComponent.CommonComponent.Groupable
{
    /// <summary>
    /// 将面板分组，使得该组中只有且总有一个组件处于特殊状态，其余组件处于默认状态
    /// </summary>
    public class AsGroupablePanel: AsGroupable
    {
        /// <summary>
        /// 当前脚本上的面板组件
        /// </summary>
        private AsPanel panel;

        /// <summary>
        /// 初始化时查找面板组件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            panel = GetComponent<AsPanel>();
        }

        /// <summary>
        /// 特殊化时调用进入方法
        /// </summary>
        public override void ToSpecial()
        {
            if (SpecializeInGroup())
            {
                panel.OnEnter();
            }
        }

        /// <summary>
        /// 取消特殊化时调用退出方法
        /// </summary>
        protected override void ToNormal()
        {
            panel.OnExit();
        }
    }
}
