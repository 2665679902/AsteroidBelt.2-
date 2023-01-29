using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Event
{
    /// <summary>
    /// 一个事件主题
    /// </summary>
    public sealed class AsEventSubject
    {
        /// <summary>
        /// 私有构建
        /// </summary>
        /// <param name="id">事件主题</param>
        internal AsEventSubject(string id) { Id = id; }

        /// <summary>
        /// 事件名称, 该名称是唯一的
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// 触发一个事件的无参方法
        /// </summary>
        public void Trigger()
        {
            foreach(var item in AsEventData.GetObserver(Id))
            {
                item.DoIt(null);
            }
        }

        /// <summary>
        /// 触发一个事件的无参方法和可以使用对应参数的方法
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="data">传入参数</param>
        /// <returns>返回传入参数使用之后的值, 如果值丢失则会返回 default</returns>
        public T Trigger<T>(T data)
        {
            var result = Trigger(data, typeof(T));

            if (result is T t)
            {
                return t;
            }

            return default;
        }

        /// <summary>
        /// 触发一个事件的无参方法和可以使用对应参数的方法
        /// </summary>
        /// <param name="data">传入参数</param>
        /// <param name="parameterType">参数类型</param>
        /// <returns>返回传入参数使用之后的值, 如果值丢失则会返回 null</returns>
        public object Trigger(object data, Type parameterType)
        {
            if (data == null)
            {
                Trigger();

                return data;
            }

            object para = data;

            foreach (var item in AsEventData.GetObserver(Id, parameterType))
            {
                para = item.DoIt(para);
            }

            return para;
        }
    }
}
