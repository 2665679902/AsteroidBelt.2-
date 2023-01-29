using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Component.EventComponent
{
    /// <summary>
    /// 事件信息数据
    /// </summary>
    public class AsComponentEventArg
    {
        /// <summary>
        /// 信息发送者
        /// </summary>
        public UnityEngine.Component Sender { get; set; }

        /// <summary>
        /// 信息数据
        /// </summary>
        public object Data { get; set; }
    }
}
