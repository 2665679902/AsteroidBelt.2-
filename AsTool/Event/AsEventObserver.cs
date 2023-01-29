using AsTool.Assert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace AsTool.Event
{
    /// <summary>
    /// 事件观察者
    /// </summary>
    public sealed class AsEventObserver
    {
        /// <summary>
        /// 观察者的名字, 并不影响观察者的订阅主题, 仅作为debug方便使用
        /// </summary>
        public string Name { get; set; } = "observer";

        /// <summary>
        /// 本观察者的参数类型, 如果事件传入参数不可被分配, 则此观察者持有的方法不会被触发
        /// </summary>
        public Type ParameterType { get; internal set; }

        /// <summary>
        /// 本观察者的行为
        /// </summary>
        private Func<object, object> _behavior;

        /// <summary>
        /// 返回本观察者是否被废弃
        /// </summary>
        public bool Abandoned { get; private set; } = false;

        /// <summary>
        /// 置空本观察者
        /// </summary>
        public void SetNull()
        {
            ParameterType = null;

            _behavior = null;
        }

        /// <summary>
        /// 废弃本观察者, 此操作不可逆
        /// </summary>
        public void AbandonObserver()
        {
            Abandoned = true;
        }

        /// <summary>
        /// 设置行为
        /// </summary>
        /// <param name="action">要设置的行为</param>
        /// <param name="catchException">是否捕获行为导致的异常(默认捕获)</param>
        public void SetBehavior(Action action, bool catchException = true)
        {
            AsAssert.NotNull(action, "AsObserver's action can not be null, maybe you want to use SetNull?");

            object behavior(object obj)
            {
                try
                {
                    action.Invoke();
                }
                catch(Exception ex)
                {
                    AsLog.Error($"AsObserver:{Name} catch exception: " + ex);

                    if(!catchException)
                        throw ex;
                }

                return obj;
            }

            _behavior = behavior;

            ParameterType = null;
        }

        /// <summary>
        /// 设置行为
        /// </summary>
        /// <typeparam name="T">行为可以传入任意参数(不建议使用结构体, 拆\装 箱会导致大量额外的消耗)</typeparam>
        /// <param name="action">要设置的行为</param>
        /// <param name="catchException">是否捕获行为导致的异常(默认捕获)</param>
        public void SetBehavior<T>(Action<T> action, bool catchException = true)
        {
            AsAssert.NotNull(action, "AsObserver's action can not be null, maybe you want to use SetNull?");

            object behavior(object obj)
            {
                try
                {
                    if (obj is null)
                        action.Invoke(default);
                    else
                        action.Invoke((T)obj);
                }
                catch (Exception ex)
                {
                    AsLog.Error($"AsObserver:{Name} catch exception: " + ex);

                    if (!catchException)
                        throw ex;
                }

                return obj;
            }

            _behavior = behavior;

            ParameterType = typeof(T);
        }

        /// <summary>
        /// 设置行为
        /// </summary>
        /// <typeparam name="T">行为可以传入任意参数(不建议使用结构体, 拆\装 箱会导致大量额外的消耗)</typeparam>
        /// <param name="function">要设置的行为, 行为返回参数就会被传递给之后的行为</param>
        /// <param name="catchException">是否捕获行为导致的异常(默认捕获)</param>
        public void SetBehavior<T>(Func<T,T> function, bool catchException = true)
        {
            AsAssert.NotNull(function, "AsObserver's function can not be null, maybe you want to use SetNull?");

            object behavior(object obj)
            {
                try
                {
                    if (obj is null)
                        return function.Invoke(default);
                    else
                        return function.Invoke((T)obj);
                }
                catch (Exception ex)
                {
                    AsLog.Error($"AsObserver:{Name} catch exception: " + ex);

                    if (!catchException)
                        throw ex;
                }

                return obj;
            }

            _behavior = behavior;

            ParameterType = typeof(T);
        }

        /// <summary>
        /// 执行行为, 如果行为被放弃, 则不会再执行
        /// </summary>
        /// <param name="obj">数据</param>
        /// <returns>返回数据</returns>
        internal object DoIt(object obj)
        {
            if (Abandoned)
                return obj;

            return _behavior?.Invoke(obj) ?? obj;
        }

        /// <summary>
        /// 私有构建
        /// </summary>
        internal AsEventObserver()
        {
        }
    }
}
