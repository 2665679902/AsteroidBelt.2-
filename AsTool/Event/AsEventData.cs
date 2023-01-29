using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Event
{
    internal static class AsEventData
    {
        /// <summary>
        /// 内部持有的所有事件主题
        /// </summary>
        private static readonly Dictionary<string, AsEventSubject> _AsSubjects = new Dictionary<string, AsEventSubject>();

        /// <summary>
        /// 内部持有的所有事件观察者
        /// </summary>
        private static readonly Dictionary<string, List<AsEventObserver>> _AsObservers = new Dictionary<string, List<AsEventObserver>>();

        /// <summary>
        /// 保证事件信息线程安全的线程锁
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// 根据事件id名称获取主题
        /// </summary>
        /// <param name="id">id名称</param>
        /// <returns>目标主题</returns>
        public static AsEventSubject GetSubject(string id)
        {
            lock (_lock)
            {
                if(!_AsSubjects.TryGetValue(id, out var value))
                {
                    _AsSubjects.Add(id, value = new AsEventSubject(id));
                }

                return value;
            }
        }

        /// <summary>
        /// 根据事件id名称获取所有订阅者
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="parameterType">目标事件的参数类型, 不可为null, 会返回所有可触发的观察者和无参观察者</param>
        /// <returns>所有符合要求的观察者列表的浅拷贝</returns>
        public static List<AsEventObserver> GetObserver(string id, Type parameterType) 
        {
            lock (_lock)
            {
                List<AsEventObserver> result = new List<AsEventObserver>();

                List<AsEventObserver> observersNeedRemove = new List<AsEventObserver>();

                if (!_AsObservers.TryGetValue(id, out var value))
                {
                    _AsObservers.Add(id, value = new List<AsEventObserver>());
                }

                for (int i=0; i < value.Count; i++)
                {
                    var observer = value[i];

                    if(observer.Abandoned)
                    {
                        observersNeedRemove.Add(observer);
                    }
                    else if(observer.ParameterType is null || parameterType.IsAssignableFrom(observer.ParameterType))
                    {
                        result.Add(observer);
                    }
                }

                foreach(var observer in observersNeedRemove)
                {
                    value.Remove(observer);
                }

                return result;
            }
        }

        /// <summary>
        /// 根据事件id名称获取所有无参订阅者
        /// </summary>
        /// <param name="id">事件id</param>
        /// <returns>所有符合要求的观察者列表的浅拷贝</returns>
        public static List<AsEventObserver> GetObserver(string id)
        {
            lock (_lock)
            {
                List<AsEventObserver> result = new List<AsEventObserver>();

                List<AsEventObserver> observersNeedRemove = new List<AsEventObserver>();

                if (!_AsObservers.TryGetValue(id, out var value))
                {
                    _AsObservers.Add(id, value = new List<AsEventObserver>());
                }

                for (int i = 0; i < value.Count; i++)
                {
                    var observer = value[i];

                    if (observer.Abandoned)
                    {
                        observersNeedRemove.Add(observer);
                    }
                    else if (observer.ParameterType is null)
                    {
                        result.Add(observer);
                    }
                }

                foreach (var observer in observersNeedRemove)
                {
                    value.Remove(observer);
                }

                return result;
            }
        }

        /// <summary>
        /// 获取一个事件信息的订阅者
        /// </summary>
        /// <param name="id">要订阅的事件</param>
        /// <returns>一个新的事件订阅者</returns>
        public static AsEventObserver GetNewAsObserver(string id)
        {
            lock (_lock)
            {
                if (!_AsObservers.TryGetValue(id, out var value))
                {
                    _AsObservers.Add(id, value = new List<AsEventObserver>());
                }

                var result = new AsEventObserver();
                value.Add(result);

                return result;
            }
        }
    }
}
