using AsTool.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Log.LogUtility
{
    /// <summary>
    /// 初始化时构建文件
    /// </summary>
    internal static class LogCore
    {
        /// <summary>
        /// 初始化时构建文件
        /// </summary>
        static LogCore()
        {
            string path = @"AsLog\Log.log";

            Trace.Listeners.Add(new TextWriterTraceListener(AsIOConfig.GetLoaclFullPath(path)));
            Trace.AutoFlush = true;

            AsFileManager.Local.Touch(path);
            AsFileManager.Local.FileDestory(path);
        }
        /// <summary>
        /// 进行Log行为
        /// </summary>
        /// <param name="logSetter">当前设置</param>
        /// <param name="message">传入信息</param>
        /// <param name="callerName">调用的函数的名字</param>
        /// <param name="fileName">调用的文件地址</param>
        /// <param name="line">当前行数</param>
        /// <param name="level">Log等级</param>
        public static void DoLog(ref LogSetter logSetter, in string message, string callerName, string fileName, int line, LogLevel level)
        {
            //如果当前的log许可等级较高 或者 log被指示不可用
            if (!logSetter.IsAble || logSetter.Level > level)
            {
                return;
            }

            var data = new LogMessage(message, callerName, fileName, line, level, logSetter.NeedTime, logSetter.NeedDetail);

            Trace.WriteLine(data.Formate2String());

            AsLog.TriggerEvent(data);
        }
    }
}
