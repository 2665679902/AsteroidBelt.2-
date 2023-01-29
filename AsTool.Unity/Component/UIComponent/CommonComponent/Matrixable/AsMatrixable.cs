using AsTool.Unity.Component.UIComponent.CommonComponent.Changable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Component.UIComponent.CommonComponent.Matrixable
{
    /// <summary>
    /// 可以自动生成子对象的自布局脚本
    /// </summary>
    public class AsMatrixable : AsAdaptable
    {
        /// <summary>
        /// 克隆的子项
        /// </summary>
        [Tooltip("要克隆的子项")]
        public GameObject ChrildObject;

        /// <summary>
        /// 脚本管理的子项列表
        /// </summary>
        [Tooltip("脚本管理的子项列表")]
        public List<GameObject> Children = new List<GameObject>();

        /// <summary>
        /// 添加一个子项
        /// </summary>
        /// <param name="callbackAfterBuild">添加前处理</param>
        public void Add(Action<GameObject> callbackAfterBuild = null)
        {
            var child = Instantiate(ChrildObject);

            child.transform.SetParent(gameObject.transform);

            Children.Add(child);

            callbackAfterBuild?.Invoke(child);
        }

        /// <summary>
        /// 移除一个子项
        /// </summary>
        /// <param name="obj">要移除的子项</param>
        /// <param name="callbackBeforeDestory">销毁前处理</param>
        /// <returns>是否找到子项</returns>
        public bool Remove(GameObject obj, Action<GameObject> callbackBeforeDestory = null)
        {
            if (!Children.Remove(obj))
            {
                return false;
            }

            callbackBeforeDestory?.Invoke(obj);

            Destroy(obj);

            return true;
        }

        /// <summary>
        /// 移除所有子项
        /// </summary>
        /// <param name="callbackBeforeDestory">销毁前处理</param>
        public void RemoveAll(Action<GameObject> callbackBeforeDestory = null)
        {
            foreach (var child in Children)
            {
                callbackBeforeDestory?.Invoke(child);

                Destroy(child);
            }

            Children.Clear();
        }
    }
}
