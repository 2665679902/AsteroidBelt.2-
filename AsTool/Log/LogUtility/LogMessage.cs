using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Log.LogUtility
{
    /// <summary>
    /// Log信息载体
    /// </summary>
    public readonly struct LogMessage
    {

        /// <summary>
        /// 消息的具体内容
        /// </summary>
        public readonly string content;

        /// <summary>
        /// 调用方法的名字
        /// </summary>
        public readonly string callerName;

        /// <summary>
        /// 调用的文件名
        /// </summary>
        public readonly string fileName;

        /// <summary>
        /// 调用代码所在行
        /// </summary>
        public readonly int line;

        /// <summary>
        /// 该信息等级
        /// </summary>
        public readonly LogLevel level;

        /// <summary>
        /// 是否需要时间
        /// </summary>
        public readonly bool NeedTime;

        /// <summary>
        /// 是否需要细节
        /// </summary>
        public readonly bool NeedDetail;

        /// <summary>
        /// 构建信息方法
        /// </summary>
        /// <param name="content">信息主体内容</param>
        /// <param name="callerName">调用的函数</param>
        /// <param name="fileName">调用文件的名字</param>
        /// <param name="line">调用行</param>
        /// <param name="level">log等级</param>
        /// <param name="needTime">是否需要时间</param>
        /// <param name="needDetail">是否需要细节</param>
        public LogMessage(string content, string callerName, string fileName, int line, LogLevel level, bool needTime, bool needDetail)
        {
            this.content = content;
            this.callerName = callerName;
            this.fileName = fileName;
            this.line = line;
            this.level = level;
            NeedTime = needTime;
            NeedDetail = needDetail;
        }

        /// <summary>
        /// 将信息转化为字符串
        /// </summary>
        /// <param name="richText">是否是富文本格式</param>
        /// <returns>转换结果</returns>
        public string Formate2String(bool richText = false)
        {
            if (richText)
                return RichFormate();
            else
                return NormalFormate();
        }

        private string RichFormate()
        {
            StringBuilder msg = new StringBuilder();
            if (NeedTime)
                msg.Append(System.DateTime.Now.ToString() + " ");

            switch (level)
            {
                case LogLevel.Debug:
                    msg.Append($"[ Debug ] -> ");
                    break;
                case LogLevel.Infor:
                    msg.Append($"[<color=blue> Info </color>] -> ");
                    break;
                case LogLevel.Error:
                    msg.Append($"[<color=orange> Error </color>] -> ");
                    break;
                case LogLevel.Fatal:
                    msg.Append($"[<color=red> Fatal </color>] -> ");
                    break;
                default:
                    msg.Append($"[<color=black> ? </color>] -> ");
                    break;
            }

            if (NeedDetail)
                msg.Append($"{fileName} : {callerName}() : in line[{line,3}]: ");
            msg.Append(content);

            return msg.ToString();
        }

        private string NormalFormate()
        {
            StringBuilder msg = new StringBuilder();
            if (NeedTime)
                msg.Append(System.DateTime.Now.ToString() + " ");
            msg.Append($"[ {level} ]  ");
            if (NeedDetail)
                msg.Append($"{fileName} : {callerName}() : in line[{line,3}]: ");
            msg.Append(content);

            return msg.ToString();
        }

        /// <summary>
        /// 默认转换为普通的格式
        /// </summary>
        /// <returns>内部信息</returns>
        public override string ToString()
        {
            return NormalFormate();
        }
    }
}
