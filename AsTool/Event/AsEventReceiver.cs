using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Event
{
    /// <summary>
    /// 事件接收者基类, 拥有此基类的类型必须拥有一个无参构造, 在加载时会生成此类型实例以实现注册
    /// </summary>
    public abstract class AsEventReceiver
    {
        /// <summary>
        /// 要注册的Id
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// 接收到事件信息之后的执行行为
        /// </summary>
        /// <param name="data">事件信息</param>
        /// <returns>修改后的事件信息</returns>
        public abstract object Action(object data);

        /// <summary>
        /// 本事件接收者的参数类型, 如果事件传入参数不可被分配, 则此观察者持有的方法不会被触发
        /// </summary>
        public abstract Type ParameterType { get; }
    }

    /// <summary>
    /// 事件接收者基类, 拥有此基类的类型必须拥有一个无参构造, 在加载时会生成此类型实例以实现注册
    /// </summary>
    /// <typeparam name="T">本事件接收者的参数类型, 如果事件传入参数不可被分配, 则此观察者持有的方法不会被触发</typeparam>
    public abstract class AsEventReceiver<T>: AsEventReceiver
    {
        /// <summary>
        /// 默认参数类型为输入的参数类型
        /// </summary>
        public override Type ParameterType => typeof(T);

        /// <summary>
        /// 接收到事件信息之后的执行行为
        /// </summary>
        /// <param name="data">事件信息</param>
        /// <returns>修改后的事件信息</returns>
        public abstract T Action(T data);

        /// <summary>
        /// 默认输出新函数的封装
        /// </summary>
        /// <param name="data">事件信息</param>
        /// <returns>修改后的事件信息</returns>
        public override object Action(object data)
        {
            return Action((T)data);
        }
    }
}
