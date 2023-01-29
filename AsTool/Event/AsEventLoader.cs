using AsTool.Load;
using AsTool.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Event
{
    /// <summary>
    /// 事件接收者加载工具
    /// </summary>
    public static class AsEventLoader
    {
        /// <summary>
        /// 所有由自动加载项注册的事件, 名字就是类型名
        /// </summary>
        public static readonly List<AsEventObserver> EventObserver = new List<AsEventObserver>();

        /// <summary>
        /// 加载所有事件接收者和转发者, 此函数不要手动调用
        /// </summary>
        [AsLoad]
        internal static void Load() 
        {
            var assemblies = AsLoadManager.AssemblieLoaded;

            List<AsType> list= new List<AsType>();

            foreach(var assembly in assemblies)
            {
                list.AddRange(AsType.GetAnyAsTypesFrom(assembly, new Type[] { typeof(AsEventReceiver)}));
            }

            foreach(var type in list)
            {
                var item = type.CompelInit();

                if(item is null)
                {
                    continue;
                }

                if(item is AsEventReceiver receiver)
                {
                    var observer = AsEvent.Subscribe(receiver.Id);
                    observer.Name = item.GetType().Name;

                    observer.SetBehavior<object>(receiver.Action);
                    observer.ParameterType = receiver.ParameterType;

                    EventObserver.Add(observer);
                }
            }
        }
    }
}
