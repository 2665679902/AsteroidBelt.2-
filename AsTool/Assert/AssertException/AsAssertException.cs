using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Assert.AssertException
{
    /// <summary>
    /// 所有AsAssert的错误的基类
    /// </summary>
    public class AsAssertException : Exception
    {
        /// <summary>
        /// 构建时传入错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        public AsAssertException(string message) : base(message)
        {
        }
    }
}
