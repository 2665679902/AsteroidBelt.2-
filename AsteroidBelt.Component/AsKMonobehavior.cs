using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EventSystem;

namespace AsteroidBelt.Component
{
    /// <summary>
    /// ONI的基础脚本的封装
    /// </summary>
    public class AsKMonobehavior: KMonoBehaviour
    {
        /// <summary>
        /// 通过哈希值注册一个事件, 这样略慢, 但是看着更方便
        /// </summary>
        /// <typeparam name="ComponentType">注册的脚本必须是此基类的子项</typeparam>
        /// <param name="gameHashes">事件类型</param>
        /// <param name="intraObjectHandler">事件处理函数</param>
        /// <returns>当前处理者的序列号</returns>
        public int Subscribe<ComponentType>(GameHashes gameHashes, IntraObjectHandler<ComponentType> intraObjectHandler) where ComponentType: AsKMonobehavior
        {
            return Subscribe((int)gameHashes, intraObjectHandler);
        }
    }
}
