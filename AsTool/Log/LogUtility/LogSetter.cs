using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Log.LogUtility
{
    /// <summary>
    /// 用于<see cref="AsLog"/>的设置，可以加载此设置以更改<see cref="AsLog"/>状态
    /// </summary>
    public sealed class LogSetter
    {
        /// <summary>
        /// 构建一个设置者
        /// </summary>
        /// <param name="level">Log的允许记录等级</param>
        /// <param name="needTime">是否要记录时间</param>
        /// <param name="needDetail">是否要记录细节</param>
        /// <param name="isAble">是否允许log记录</param>
        public LogSetter(LogLevel level = LogLevel.Debug, bool needTime = false, bool needDetail = false, bool isAble = true)
        {
            Level = level;
            NeedTime = needTime;
            NeedDetail = needDetail;
            IsAble = isAble;
        }

        /// <summary>
        /// 当前log许可等级，等级低于当前的log不会被触发
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// 是否需要显示时间
        /// </summary>
        public bool NeedTime { get; }

        /// <summary>
        /// 是否需要显示细节
        /// </summary>
        public bool NeedDetail { get; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsAble { get; }

        /// <summary>
        /// 转换为字符串的方法，方便debug
        /// </summary>
        /// <returns>转换结果</returns>
        public override string ToString()
        {
            return $"AsLogSetter:  Level:{Level}  NeedTime{NeedTime}  NeedDetail{NeedDetail}  IsAble{IsAble}";
        }
    }
}
