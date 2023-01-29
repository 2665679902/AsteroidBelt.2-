using AsTool.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Load
{
    /// <summary>
    /// 用于执行所有需要加载的无参静态方法
    /// </summary>
    public static class AsLoadManager
    {
        /// <summary>
        /// 用于记载加载项的单个类
        /// </summary>
        private class LoadMethodItem : IComparable<LoadMethodItem>
        {
            /// <summary>
            /// 加载优先级，优先级越高， 加载顺序越靠前
            /// </summary>
            public readonly int Priority;

            /// <summary>
            /// 方法信息
            /// </summary>
            public readonly AsType.AsMemberInfo Info;

            /// <summary>
            /// 构建一个加载项
            /// </summary>
            /// <param name="info">成员信息</param>
            public LoadMethodItem(AsType.AsMemberInfo info)
            {
                Info = info;

                if (Info.TryGetCustomAttribute(typeof(AsLoadAttribute), out var attribute))
                {
                    Priority = (attribute as AsLoadAttribute)?.Priority ?? 5;
                }
            }

            public void TryToInvokeMethod()
            {
                if (!Info.IsStatic || !Info.IsMethod)
                    return;

                try
                {
                    Info.InvokeMethod(null);
                }
                catch (Exception ex)
                {
                    AsLog.Error("AsLoadManager Load failed beacuse: " + ex);
                }
            }

            public int CompareTo(LoadMethodItem other)
            {
                return Priority.CompareTo(other.Priority);
            }
        }

        /// <summary>
        /// 是否已经加载完成
        /// </summary>
        public static bool Loaded { get; private set; } = false;

        /// <summary>
        /// 返回所有将会被加载的程序集, 如果还没加载则返回 null
        /// </summary>
        public static Assembly[] AssemblieLoaded { get; private set; }

        /// <summary>
        /// 执行所有需要加载的函数, 如果已经执行过一次了, 则此函数不会生效
        /// </summary>
        public static void StartLoad()
        {
            if (Loaded) return; Loaded = true;

            var path = Assembly.GetExecutingAssembly().Location.Replace("AsTool.dll", "");

            var MethodList = new List<LoadMethodItem>();

            var assemblieLoaded = new List<Assembly>();

            foreach (var dll in Directory.GetFiles(path).Where(s => s.EndsWith(".dll")))
            {
                var assembly = Assembly.LoadFrom(dll);

                assemblieLoaded.Add(assembly);

                AsLog.Info($"程序集捕获 get assembly {assembly.GetName().Name}");

                var result = AsType.GetMemberInfoFrom(assembly, typeof(AsLoadAttribute)).Select(info => new LoadMethodItem(info));

                if (result.Any())
                {
                    MethodList.AddRange(result);
                }
            }

            AssemblieLoaded = assemblieLoaded.ToArray();

            //升序排列
            MethodList.Sort();

            for(int i = MethodList.Count -1; i >= 0; i--)
            {
                MethodList[i].TryToInvokeMethod();
            }
        }
    }
}
