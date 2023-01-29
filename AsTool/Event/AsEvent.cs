using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Event
{
    /// <summary>
    /// 用于触发和管理事件的工具类
    /// </summary>
    public static class AsEvent
    {
        /// <summary>
        /// 订阅一个事件, 获取目标事件订阅者
        /// </summary>
        /// <param name="id">事件名称</param>
        /// <returns>返回一个事件观察者</returns>
        public static AsEventObserver Subscribe(string id)
        {
            return AsEventData.GetNewAsObserver(id);
        }

        /// <summary>
        /// 订阅一个事件, 获取目标事件订阅者
        /// </summary>
        /// <param name="id">事件名称</param>
        /// <param name="action">要设置的行为</param>
        /// <returns>返回一个事件观察者</returns>
        public static AsEventObserver Subscribe(string id, Action action)
        {
            var result = Subscribe(id);

            result.SetBehavior(action);

            return result;
        }

        /// <summary>
        /// 订阅一个事件, 获取目标事件订阅者
        /// </summary>
        /// <param name="id">事件名称</param>
        /// <param name="action">要设置的行为</param>
        /// <returns>返回一个事件观察者</returns>
        public static AsEventObserver Subscribe<T>(string id, Action<T> action)
        {
            var result = Subscribe(id);

            result.SetBehavior(action);

            return result;
        }

        /// <summary>
        /// 订阅一个事件, 获取目标事件订阅者
        /// </summary>
        /// <param name="id">事件名称</param>
        /// <param name="function">要设置的行为, 行为返回值将作为下一个行为的传入值</param>
        /// <returns>返回一个事件观察者</returns>
        public static AsEventObserver Subscribe<T>(string id, Func<T,T> function)
        {
            var result = Subscribe(id);

            result.SetBehavior(function);

            return result;
        }

        /// <summary>
        /// 触发一个事件的无参方法
        /// </summary>
        /// <param name="id">事件id</param>
        public static void Trigger(string id)
        {
            AsEventData.GetSubject(id).Trigger();
        }

        /// <summary>
        /// 触发一个事件的无参方法和可以使用对应参数的方法
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="id">事件id</param>
        /// <param name="data">传入参数</param>
        /// <returns>返回传入参数使用之后的值, 如果值丢失则会返回 default</returns>
        public static T Trigger<T>(string id, T data)
        {
            return AsEventData.GetSubject(id).Trigger(data);
        }

        /// <summary>
        /// 获取一个事件主题
        /// </summary>
        /// <param name="id">事件名称</param>
        /// <returns>目标主题</returns>
        public static AsEventSubject GetAsSubject(string id)
        {
            return AsEventData.GetSubject(id);
        }
    }
}
