using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace AsTool.Unity.Component.UIComponent.CommonComponent.Changable
{
    /// <summary>
    /// 自动添加默认属性的自动布局组件以帮助UI适应子对象
    /// </summary>
    public class AsAdaptable : AsMonoBehaviour
    {
        /// <summary>
        /// 默认的组件间间隔
        /// </summary>
        [Tooltip("默认的组件间间隔")]
        public int DefaultSpacing = 10;

        /// <summary>
        /// 默认的组件边距
        /// </summary>
        [Tooltip("默认的组件边距")]
        public int DefaultPadding = 10;

        /// <summary>
        /// 默认的布局方法
        /// </summary>
        [Tooltip("默认的布局方法")]
        public LayOutMethod layOutMethod = LayOutMethod.Vertical;

        /// <summary>
        /// 默认的排列方式
        /// </summary>
        [Header("如果使用格子布局, 以下参数生效")]
        [Tooltip("如果使用格子布局, 则使用的排列方式")]
        public GridLayoutGroup.Constraint Constraint = GridLayoutGroup.Constraint.Flexible;

        /// <summary>
        /// 如果使用固定布局, 每列或每行的参数
        /// </summary>
        [Tooltip("如果使用固定布局, 每列或每行的参数")]
        public int constraintCount = 3;

        /// <summary>
        /// 管理的自动垂直布局组件
        /// </summary>
        private VerticalLayoutGroup _verticalLayoutGroup;

        /// <summary>
        /// 管理的水平自动布局组件
        /// </summary>
        private HorizontalLayoutGroup _horizontalLayoutGroup;

        /// <summary>
        /// 管理的格子自动布局组件
        /// </summary>
        private GridLayoutGroup _gridLayoutGroup;

        /// <summary>
        /// 管理适配内容的组件
        /// </summary>
        private ContentSizeFitter _contentSizeFitter;

        /// <summary>
        /// 指示此时是否需要刷新
        /// </summary>
        private bool NeedRefresh
        {
            get
            {
                return
                    !((_verticalLayoutGroup != null && layOutMethod == LayOutMethod.Vertical) ||
                    (_horizontalLayoutGroup != null && layOutMethod == LayOutMethod.Horizontal) ||
                    (_gridLayoutGroup != null && layOutMethod == LayOutMethod.Grid) ||
                    (_verticalLayoutGroup is null && _horizontalLayoutGroup is null && layOutMethod == LayOutMethod.None));
            }
        }

        /// <summary>
        /// 在生成时调用
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            Refresh();

            _contentSizeFitter = GetOrAddComponent<ContentSizeFitter>();
            _contentSizeFitter.horizontalFit = _contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        /// <summary>
        /// 在帧更新时检查是否需要改变布局状态
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (NeedRefresh)
            {
                Refresh();
            }

        }

        /// <summary>
        /// 设置默认的垂直布局组件
        /// </summary>
        protected void SetDefaultVerticalLayoutGroup()
        {
            //管理组件状态
            _verticalLayoutGroup = GetOrAddComponent<VerticalLayoutGroup>();
            _horizontalLayoutGroup = null;
            _ = TryRemoveComponent<HorizontalLayoutGroup>();
            _gridLayoutGroup = null;
            _ = TryRemoveComponent<GridLayoutGroup>();

            //设置默认边距
            _verticalLayoutGroup.spacing = DefaultSpacing;
            _verticalLayoutGroup.padding.right = _verticalLayoutGroup.padding.left = _verticalLayoutGroup.padding.top = _verticalLayoutGroup.padding.bottom = DefaultPadding;

            //添加默认设置

            //子组件居中
            _verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

            //是否控制子项长宽以填充父对象
            _verticalLayoutGroup.childControlHeight = _verticalLayoutGroup.childControlWidth = false;

            //是否考虑子项的缩放
            _verticalLayoutGroup.childScaleHeight = _verticalLayoutGroup.childScaleWidth = true;

            //是否强制延长子项间隔以适配父对象
            _verticalLayoutGroup.childForceExpandHeight = _verticalLayoutGroup.childForceExpandWidth = false;
        }

        /// <summary>
        /// 设置默认的水平布局组件
        /// </summary>
        protected void SetDefaultHorizontalLayoutGroup()
        {
            //管理组件状态
            _horizontalLayoutGroup = GetOrAddComponent<HorizontalLayoutGroup>();
            _verticalLayoutGroup = null;
            _ = TryRemoveComponent<VerticalLayoutGroup>();
            _gridLayoutGroup = null;
            _ = TryRemoveComponent<GridLayoutGroup>();

            //设置默认边距
            _horizontalLayoutGroup.spacing = DefaultSpacing;
            _horizontalLayoutGroup.padding.right = _horizontalLayoutGroup.padding.left = _horizontalLayoutGroup.padding.top = _horizontalLayoutGroup.padding.bottom = DefaultPadding;

            //添加默认设置

            //子组件居中
            _horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

            //是否控制子项长宽以填充父对象
            _horizontalLayoutGroup.childControlHeight = _horizontalLayoutGroup.childControlWidth = false;

            //是否考虑子项的缩放
            _horizontalLayoutGroup.childScaleHeight = _horizontalLayoutGroup.childScaleWidth = true;

            //是否强制延长子项间隔以适配父对象
            _horizontalLayoutGroup.childForceExpandHeight = _horizontalLayoutGroup.childForceExpandWidth = false;
        }

        /// <summary>
        /// 设置默认的格子展开模式
        /// </summary>
        protected void SetDefaultGridLayoutGroup()
        {
            _gridLayoutGroup = GetOrAddComponent<GridLayoutGroup>();
            _verticalLayoutGroup = null;
            _ = TryRemoveComponent<VerticalLayoutGroup>();
            _horizontalLayoutGroup = null;
            _ = TryRemoveComponent<HorizontalLayoutGroup>();

            _gridLayoutGroup.spacing = new Vector2(DefaultSpacing, DefaultSpacing);
            _gridLayoutGroup.padding.right = _gridLayoutGroup.padding.left = _gridLayoutGroup.padding.top = _gridLayoutGroup.padding.bottom = DefaultPadding;

            //子组件居中
            _gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

            _gridLayoutGroup.constraint = Constraint;
            _gridLayoutGroup.constraintCount = constraintCount;
        }

        /// <summary>
        /// 设置仅使用大小自适应子项
        /// </summary>
        public void SetNone()
        {
            _horizontalLayoutGroup = null;
            _ = TryRemoveComponent<HorizontalLayoutGroup>();

            _verticalLayoutGroup = null;
            _ = TryRemoveComponent<VerticalLayoutGroup>();
        }

        /// <summary>
        /// 刷新当前脚本的状态
        /// </summary>
        public void Refresh()
        {
            switch (layOutMethod)
            {
                case LayOutMethod.Vertical:
                    SetDefaultVerticalLayoutGroup();
                    break;

                case LayOutMethod.Horizontal:
                    SetDefaultHorizontalLayoutGroup();
                    break;

                case LayOutMethod.Grid:
                    SetDefaultGridLayoutGroup();
                    break;

                case LayOutMethod.None:
                    SetNone();
                    break;
            }
        }

        /// <summary>
        /// 布局方法
        /// </summary>
        public enum LayOutMethod
        {
            /// <summary>
            /// 仅使用大小自适应子项
            /// </summary>
            [Tooltip("仅使用大小自适应子项")]
            None,

            /// <summary>
            /// 垂直
            /// </summary>
            [Tooltip("垂直")]
            Vertical,

            /// <summary>
            /// 水平
            /// </summary>
            [Tooltip("水平")]
            Horizontal,

            /// <summary>
            /// 格子模式
            /// </summary>
            [Tooltip("格子模式")]
            Grid
        }
    }
}
