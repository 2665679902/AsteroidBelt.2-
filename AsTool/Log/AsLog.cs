using AsTool.IO;
using AsTool.Log.LogUtility;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// 日志工具类
/// </summary>
public static class AsLog
{
    /// <summary>
    /// 存放本地Log的文件夹
    /// </summary>
    public static string LogFilePath { get => AsIOConfig.GetLoaclFullPath("AsLog"); }

    /// <summary>
    /// log的线程锁
    /// </summary>
    private static readonly object _lock = new object();

    #region Log属性设置

    /// <summary>
    /// 当前的Log设置
    /// </summary>
    private static LogSetter _setter = new LogSetter();

    /// <summary>
    /// 加载一个新Log设置，线程安全
    /// </summary>
    /// <param name="setter">新设置</param>
    public static void LoadSetter(LogSetter setter)
    {
        lock (_lock)
        {
            _setter = setter;
        }
    }

    #endregion

    /// <summary>
    /// 当Log事件发生时，此委托会被调用
    /// </summary>
    public static event Action<LogMessage> Logging;

    /// <summary>
    /// 写入一条Debug信息
    /// </summary>
    /// <param name="message">要写入的信息</param>
    /// <param name="callerName">调用函数的名字</param>
    /// <param name="fileName">调用文件的名字</param>
    /// <param name="line">调用时的行数</param>
    public static void Debug(
        in object message = null,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string fileName = null,
        [CallerLineNumber] int line = 0)
    {
        var messageString = message?.ToString();

        lock (_lock)
            LogCore.DoLog(ref _setter, messageString, callerName, fileName, line, LogLevel.Debug);
    }

    /// <summary>
    /// 写入一条Infor信息
    /// </summary>
    /// <param name="message">要写入的信息</param>
    /// <param name="callerName">调用函数的名字</param>
    /// <param name="fileName">调用文件的名字</param>
    /// <param name="line">调用时的行数</param>
    public static void Info(
        in object message = null,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string fileName = null,
        [CallerLineNumber] int line = 0)
    {
        var messageString = message?.ToString();

        lock (_lock)
            LogCore.DoLog(ref _setter, messageString, callerName, fileName, line, LogLevel.Infor);
    }

    /// <summary>
    /// 写入一条Error信息
    /// </summary>
    /// <param name="message">要写入的信息</param>
    /// <param name="callerName">调用函数的名字</param>
    /// <param name="fileName">调用文件的名字</param>
    /// <param name="line">调用时的行数</param>
    public static void Error(
         in object message = null,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string fileName = null,
        [CallerLineNumber] int line = 0)
    {

        var messageString = message?.ToString();

        lock (_lock)
            LogCore.DoLog(ref _setter, messageString, callerName, fileName, line, LogLevel.Error);

    }

    /// <summary>
    /// 写入一条Fatal信息
    /// </summary>
    /// <param name="message">要写入的信息</param>
    /// <param name="callerName">调用函数的名字</param>
    /// <param name="fileName">调用文件的名字</param>
    /// <param name="line">调用时的行数</param>
    public static void Fatal(
        in object message = null,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string fileName = null,
        [CallerLineNumber] int line = 0)
    {
        var messageString = message?.ToString();

        lock (_lock)
            LogCore.DoLog(ref _setter, messageString, callerName, fileName, line, LogLevel.Fatal);
    }

    /// <summary>
    /// 程序集内部用于触发Log事件的函数(本来是用委托的， 用事件可以避免写线程锁)
    /// </summary>
    /// <param name="logMessage">Log信息</param>
    internal static void TriggerEvent(LogMessage logMessage) => Logging?.Invoke(logMessage);
}
