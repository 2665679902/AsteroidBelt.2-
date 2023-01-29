using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Component
{
    /// <summary>
    /// 所有的模组脚本的基类
    /// </summary>
    public class AsMonoBehaviour : MonoBehaviour
    {
        static AsMonoBehaviour()
        {
        }

        /// <summary>
        /// 指示脚本是否已经被初始化
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// 在引用需要初始化的内容时，确保<see cref="OnAwake"/>已经被调用
        /// </summary>
        public void ConfirmInit()
        {
            if (!initialized)
            {
                OnAwake();
            }
        }

        /// <summary>
        /// 在初始化时调用此函数, 避免使用<see cref="Awake"/>,有时候脚本会在初始化前被调用
        /// </summary>
        protected virtual void OnAwake()
        {
            initialized = true;

            //回头再优化

            foreach(var components in GetComponentsInChildren<AsMonoBehaviour>(true))
            {
                components?.ConfirmInit();
            }
        }

        /// <summary>
        /// 唤醒事件，游戏一开始运行就执行，只执行一次。
        /// </summary>
        protected void Awake()
        {
            if (!initialized)
            {
                OnAwake();
            }
        }

        /// <summary>
        /// 启用事件，只执行一次。当脚本组件被启用的时候执行一次。
        /// </summary>
        protected virtual void OnEnable()
        {

        }

        /// <summary>
        /// 开始事件，执行一次。
        /// </summary>
        protected virtual void Start()
        {

        }

        /// <summary>
        /// 固定更新事件，执行N次，0.02秒执行一次。所有物理组件相关的更新都在这个事件中处理。
        /// </summary>
        protected virtual void FixedUpdate()
        {

        }

        /// <summary>
        /// 更新事件，执行N次，每帧执行一次。
        /// </summary>
        protected virtual void Update()
        {

        }

        /// <summary>
        /// 稍后更新事件，执行N次，在 Update() 事件执行完毕后再执行。
        /// </summary>
        protected virtual void LateUpdate()
        {
        }

        /// <summary>
        /// GUI渲染事件，执行N次，执行的次数是 Update() 事件的两倍。
        /// </summary>
        protected virtual void OnGUI()
        {

        }

        /// <summary>
        /// 禁用事件，执行一次。在 <see cref="OnDestroy()"/> 事件前执行。或者当该脚本组件被“禁用”后，也会触发该事件。
        /// </summary>
        protected virtual void OnDisable()
        {

        }

        /// <summary>
        /// 销毁事件，执行一次。当脚本所挂载的游戏物体被销毁时执行。
        /// </summary>
        protected virtual void OnDestroy()
        {

        }

        /// <summary>
        /// 获取或添加一个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>目标类型的组件</returns>
        public T GetOrAddComponent<T>() where T : UnityEngine.Component
        {
            if (!TryGetComponent<T>(out var result))
            {
                result = gameObject.AddComponent<T>();
            }

            return result;
        }

        /// <summary>
        /// 获取或添加一个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>目标类型的组件</returns>
        public T GetOrAddComponent<T>(GameObject gameObject) where T : UnityEngine.Component
        {
            if (!gameObject.TryGetComponent<T>(out var result))
            {
                result = gameObject.AddComponent<T>();
            }

            return result;
        }

        /// <summary>
        /// 尝试移除一个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="immediate">是否立即移除(可能会卡)</param>
        /// <returns>是否找到并移除目标组件</returns>
        public bool TryRemoveComponent<T>(bool immediate = false) where T : UnityEngine.Component
        {
            if (!TryGetComponent<T>(out var result))
            {
                return false;
            }

            if (immediate)
                DestroyImmediate(gameObject);
            else
                Destroy(result);

            return true;
        }
    }
}
