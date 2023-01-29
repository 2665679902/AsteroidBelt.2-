using AsTool.Unity.Component.UIComponent.CommonComponent.Draggable;
using AsTool.Unity.Component.UIComponent.CommonComponent.Groupable;
using AsTool.Unity.Component.UIComponent.CommonComponent.Locatable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Component.UIComponent.DefaultUIComponent
{
    /// <summary>
    /// 面板组件
    /// </summary>
    public class AsPanel: AsUIComponent
    {
        /// <summary>
        /// 组件默认是否可以拖动
        /// </summary>
        [Tooltip("组件默认是否可以拖动")]
        [Header("拖动设置")]
        public bool Draggable = true;

        /// <summary>
        /// 如果组件可以拖动 是否使本对象在初始化时位于屏幕中心
        /// </summary>
        [Tooltip("是否使本对象在初始化时位于屏幕中心")]
        [Header("是否在初始化时移动到屏幕中心")]
        public bool InitAtCenter = false;

        /// <summary>
        /// 组件默认是否在某个组件组中
        /// </summary>
        [Tooltip("组件默认是否在某个组件组中")]
        [Header("分组设置")]
        public bool Groupable = true;

        /// <summary>
        /// 如果默认该面板在某个组中, 则其默认的组名
        /// </summary>
        [Tooltip("如果默认该面板在某个组中, 则其默认的组名")]
        public string DefaultGroupName = "AsPanel";

        /// <summary>
        /// 如果在某个组中, 则此为初始化时激活的优先级, 优先级越高, 在初始化时越先被激活
        /// </summary>
        [Tooltip("如果在某个组中, 则此为初始化时激活的优先级, 优先级越高, 在初始化时越先被激活")]
        public int Priority = 0;

        /// <summary>
        /// 如果本脚本生成了可移动组件, 则此字段为对其的引用
        /// </summary>
        protected AsDraggableInScreen draggableInScreen;

        /// <summary>
        /// 如果本脚本生成了可定位组件或者可移动组件, 则此字段为对其的引用
        /// </summary>
        protected AsLocatable locatable;

        /// <summary>
        /// 如果本脚本生成了面板组组件, 则此字段为对其的引用
        /// </summary>
        protected AsGroupablePanel groupable;

        /// <summary>
        /// 初始化时分配组件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            if (Draggable)
            {
                draggableInScreen = GetOrAddComponent<AsDraggableInScreen>();
                locatable = draggableInScreen;

                if (InitAtCenter)
                {
                    draggableInScreen.InitAtCenter = true;

                    draggableInScreen.ConfirmInit();
                    draggableInScreen.MoveToCenter();
                }

            }
            else if(InitAtCenter)
            {
                locatable = GetOrAddComponent<AsLocatable>();
                locatable.InitAtCenter = true;
                locatable.ConfirmInit();
                locatable.MoveToCenter();
            }


            if (Groupable)
            {
                groupable = GetOrAddComponent<AsGroupablePanel>();
                groupable.ConfirmInit();

                groupable.Priority= Priority;
                groupable.SetGroupName(DefaultGroupName);
            }
        }

        /// <summary>
        /// 当进入主视野时进行的行为
        /// </summary>
        public virtual void OnEnter()
        {
            gameObject?.SetActive(true);
        }

        /// <summary>
        /// 当退出主视野时进行的行为
        /// </summary>
        public virtual void OnExit()
        {
            gameObject?.SetActive(false);
        }
    }
}
