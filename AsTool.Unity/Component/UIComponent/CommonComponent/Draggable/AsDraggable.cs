using AsTool.Unity.Component.UIComponent.CommonComponent.Locatable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace AsTool.Unity.Component.UIComponent.CommonComponent.Draggable
{
    /// <summary>
    /// 使用此组件的脚本可拖拽
    /// </summary>
    public class AsDraggable : AsLocatable, IDragHandler
    {
        /// <summary>
        /// 初始位置的差值
        /// </summary>
        private Vector2 positionDifference;

        /// <summary>
        /// 本次拖动的初始位置
        /// </summary>
        private Vector2 pressPosition;

        /// <summary>
        /// 本脚本的位置组件
        /// </summary>
        protected RectTransform obj_transform;

        /// <summary>
        /// 唤醒时初步构建
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            obj_transform = gameObject.GetComponent<RectTransform>();
        }

        /// <summary>
        /// 在被拖动时调用此函数计算位置，在派生类中重写
        /// </summary>
        /// <param name="eventData">拖动事件信息</param>
        /// <returns>位置计算结果，此次结果将被赋值给<see cref="obj_transform"/> 的 position 属性</returns>
        protected virtual Vector3 OnPositionCalculation(PointerEventData eventData)
        {
            //判断是否是同一次拖拽
            if (pressPosition != eventData.pressPosition)
            {
                pressPosition = eventData.pressPosition;
                positionDifference = new Vector2(obj_transform.position.x, obj_transform.position.y) - pressPosition;
            }

            //返回计算结果
            return eventData.position + positionDifference;
        }

        /// <summary>
        /// 在拖动时调用此代码
        /// </summary>
        /// <param name="eventData">事件数据</param>
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            obj_transform.position = OnPositionCalculation(eventData);
        }
    }
}
