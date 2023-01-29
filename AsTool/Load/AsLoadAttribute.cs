using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Load
{
    /// <summary>
    /// 需要加载的无参静态方法的标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AsLoadAttribute: Attribute
    {
        /// <summary>
        /// 加载优先级，优先级越高， 加载顺序越靠前
        /// </summary>
        public readonly int Priority;

        /// <summary>
        /// 在生成标签时确定优先级
        /// </summary>
        /// <param name="priority">优先级</param>
        public AsLoadAttribute(int priority = 5)
        {
            Priority = priority;
        }
    }
}
