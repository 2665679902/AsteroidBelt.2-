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
    /// 使目标矩形组件仅可在屏幕范围内拖动
    /// </summary>
    public sealed class AsDraggableInScreen : AsDraggable
    {
        /// <summary>
        /// 获取当前拖动后的位置、判断状态并做出相应反馈
        /// </summary>
        /// <param name="eventData">拖动事件信息</param>
        /// <returns>位置计算结果，此次结果将被赋值给<see cref="AsDraggable.obj_transform"/> 的 position 属性</returns>
        protected override Vector3 OnPositionCalculation(PointerEventData eventData)
        {
            var result = base.OnPositionCalculation(eventData);

            DealResult(ref result);

            return result;
        }

        /// <summary>
        /// 处理<see cref="OnPositionCalculation"/>计算结果的函数,使其不会移出屏幕
        /// </summary>
        /// <param name="vector3">将要平移到的位置</param>
        private void DealResult(ref Vector3 vector3)
        {
            Vector3[] corners = new Vector3[4];

            obj_transform.GetWorldCorners(corners);

            var dif = vector3 - obj_transform.position;

            var difLeft = dif.x + corners[0].x;

            if (difLeft < 0)
            {
                vector3.x -= difLeft;
            }

            var difButtom = dif.y + corners[0].y;

            if (difButtom < 0)
            {
                vector3.y -= difButtom;
            }

            var difRight = screenWidth - (dif.x + corners[2].x);

            if (difRight < 0)
            {
                vector3.x += difRight;
            }


            var difTop = screenHeight - (dif.y + corners[2].y);

            if (difTop < 0)
            {
                vector3.y += difTop;
            }
        }
    }
}
