using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.Data.EventTokens
{
    /// <summary>
    /// 存放了一些数据事件指令
    /// </summary>
    public static class DataEvents
    {
        /// <summary>
        /// 此指令会刷新模板数据
        /// </summary>
        public const string RefreshTemplateData = "DataEvents_RefreshTemplateData";
    }
}
