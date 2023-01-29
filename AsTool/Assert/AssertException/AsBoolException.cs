using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Assert.AssertException
{
    /// <summary>
    /// 断言 bool 值错误
    /// </summary>
    public class AsBoolException : AsAssertException
    {
        /// <summary>
        /// 构建时传入错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        public AsBoolException(string message) : base(message)
        {
        }
    }
}
