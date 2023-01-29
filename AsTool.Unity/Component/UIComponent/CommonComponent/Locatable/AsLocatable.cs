using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Component.UIComponent.CommonComponent.Locatable
{
    /// <summary>
    /// 一个可以定位自身在屏幕中的相对位置的脚本
    /// </summary>
    public class AsLocatable : AsMonoBehaviour
    {
        /// <summary>
        /// 是否使本对象在初始化时位于屏幕中心
        /// </summary>
        [Tooltip("是否使本对象在初始化时位于屏幕中心")]
        public bool InitAtCenter = false;

        /// <summary>
        /// 屏幕的宽
        /// </summary>
        protected readonly int screenWidth = Screen.width;

        /// <summary>
        /// 屏幕的高
        /// </summary>
        protected readonly int screenHeight = Screen.height;

        /// <summary>
        /// 在初始化时调用
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            if (InitAtCenter)
                MoveToCenter();
        }

        /// <summary>
        /// 将本脚本依附的对象移动至屏幕的中心
        /// </summary>
        public void MoveToCenter()
        {
            gameObject.transform.position = new Vector3(screenWidth / 2, screenHeight / 2, gameObject.transform.position.z);
        }
    }
}
