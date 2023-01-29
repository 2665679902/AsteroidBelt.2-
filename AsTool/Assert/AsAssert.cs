using AsTool.Assert.AssertException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Assert
{
    /// <summary>
    /// 存放了一些断言，用于检测
    /// </summary>
    public static class AsAssert
    {
        /// <summary>
        /// 抛出一个错误
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="callerName">不需要填写</param>
        /// <param name="line">不需要填写</param>
        /// <exception cref="AsAssertException">将要抛出的错误</exception>
        public static void Fatal(string message, [CallerMemberName] string callerName = null, [CallerLineNumber] int line = 0)
        {
            throw new AsAssertException($"AsAssert throw an exception from {callerName} in line {line},  message: [ {message} ]");
        }

        /// <summary>
        /// 断言对象不为Null，否则报错
        /// </summary>
        /// <typeparam name="T">要检测的对象类</typeparam>
        /// <param name="o"></param>
        /// <param name="callerName">不需要填写</param>
        /// <param name="line">不需要填写</param>
        /// <param name="message">错误信息</param>
        /// <exception cref="AsNullException">参数 null 报错</exception>
        public static void NotNull<T>(in T o, string message, [CallerMemberName] string callerName = null, [CallerLineNumber] int line = 0) where T : class
        {
            if (o is null)
                throw new AsNullException($"AsAssert Get a null object from {callerName} in line {line},  message: [ {message} ]");
        }

        /// <summary>
        /// 断言对象不为Null，否则报错
        /// </summary>
        /// <typeparam name="T">要检测的结构体</typeparam>
        /// <param name="o"></param>
        /// <param name="message">错误信息</param>
        /// <param name="callerName">不需要填写</param>
        /// <param name="line">不需要填写</param>
        /// <exception cref="AsNullException">参数 null 报错</exception>
        public static void NotNull<T>(in T? o, string message, [CallerMemberName] string callerName = null, [CallerLineNumber] int line = 0) where T : struct
        {
            if (o is null)
                throw new AsNullException($"AsAssert Get a null object from {callerName} in line {line},  message: [ {message} ]");
        }

        /// <summary>
        /// 断言此项应当为false，否则报错
        /// </summary>
        /// <param name="result">要判断的bool</param>
        /// <param name="message">错误信息</param>
        /// <param name="callerName">不需要填写</param>
        /// <param name="line">不需要填写</param>
        /// <exception cref="AsBoolException">参数 true 报错</exception>
        public static void IsFalse(bool result, string message, [CallerMemberName] string callerName = null, [CallerLineNumber] int line = 0)
        {
            if (result)
                throw new AsBoolException($"AsAssert Get a unexcepted true from {callerName} in line {line},  message: [ {message} ]");
        }

        /// <summary>
        /// 断言此项应当为true，否则报错
        /// </summary>
        /// <param name="result">要判断的bool</param>
        /// <param name="message">错误信息</param>
        /// <param name="callerName">不需要填写</param>
        /// <param name="line">不需要填写</param>
        /// <exception cref="AsBoolException">参数 true 报错</exception>
        public static void IsTrue(bool result, string message, [CallerMemberName] string callerName = null, [CallerLineNumber] int line = 0)
        {
            if (!result)
                throw new AsBoolException($"AsAssert Get a unexcepted false from {callerName} in line {line},  message: [ {message}  ]");
        }

    }
}
