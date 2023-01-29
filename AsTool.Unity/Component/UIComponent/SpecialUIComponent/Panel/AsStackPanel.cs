using AsTool.Unity.Component.UIComponent.CommonComponent.Groupable;
using AsTool.Unity.Component.UIComponent.DefaultUIComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Component.UIComponent.SpecialUIComponent.Panel
{
    /// <summary>
    /// 有栈面板组件
    /// </summary>
    public class AsStackPanel: AsPanel
    {
        /// <summary>
        /// 被隐藏时的缩进方向
        /// </summary>
        [Header("被隐藏时的缩进方向")]
        public HiddenDirection direction = HiddenDirection.Left;

        /// <summary>
        /// 当前坐标
        /// </summary>
        public string Pos;

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            base.Update();

            Pos = transform.position.ToString();
        }

        /// <summary>
        /// 所有由于隐藏面板而失活的子组件
        /// </summary>
        private readonly HashSet<GameObject> hiddenlist = new HashSet<GameObject>();

        /// <summary>
        /// 尝试获取上级栈面板
        /// </summary>
        private AsStackPanel GetParentPanel() => transform.parent?.GetComponentInParent<AsStackPanel>();

        /// <summary>
        /// 初始化时隐藏面板自身
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();
        }

        /// <summary>
        /// 在被子面板隐藏时调用
        /// </summary>
        /// <param name="child">要显示的子对象</param>
        protected virtual void OnHide(AsStackPanel child)
        {
            var rectTransform = GetComponent<RectTransform>();
            var childRectTransform = child.GetComponent<RectTransform>();

            //拼装主要面板
            switch (direction)
            {
                case HiddenDirection.Left:
                    HideToLeft(rectTransform, childRectTransform); 
                    break;
            }

            //失活其他子组件
            foreach (Transform childRect in transform)
            {
                if (childRect.gameObject.activeSelf && child.gameObject != childRect.gameObject && !child.transform.IsChildOf(childRect))
                {
                    childRect.gameObject.SetActive(false);
                    hiddenlist.Add(childRect.gameObject);
                }
            }
        }

        /// <summary>
        /// 本面板向左隐藏
        /// </summary>
        /// <param name="parent">父对象</param>
        /// <param name="child">要显示的子对象</param>
        private void HideToLeft(RectTransform parent, RectTransform child)
        {
            child.transform.position = new Vector3(parent.transform.position.x + parent.sizeDelta.x * 0.05f, parent.transform.position.y, parent.transform.position.z);
            //child.transform.position = parent.transform.position;
            child.sizeDelta = new Vector2(parent.sizeDelta.x * 0.9f, parent.sizeDelta.y);

            //AsLog.Debug(child.gameObject);
            //AsLog.Debug(parent.gameObject);

        }

        /// <summary>
        /// 所有子面板退出隐藏时调用
        /// </summary>
        protected virtual void OnExitingHide()
        {
            foreach(var obj in hiddenlist)
            {
                obj.SetActive(true);
            }

            hiddenlist.Clear();
        }

        /// <summary>
        /// 在进入主视野时调用
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();

            GetParentPanel()?.OnHide(this);
        }

        /// <summary>
        /// 在退出主视野时调用
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();

            GetParentPanel()?.OnExitingHide();
        }

        /// <summary>
        /// 面板的隐藏方向
        /// </summary>
        public enum HiddenDirection
        {
            /// <summary>
            /// 向左隐藏
            /// </summary>
            [Tooltip("向左隐藏")]
            Left,
        }
    }
}
