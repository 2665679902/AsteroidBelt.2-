using AsteroidBelt.Data.String;
using AsTool.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.Data.templates
{
    /// <summary>
    /// template基类
    /// </summary>
    public class AsTemplatesObject: AsDataObject
    {
        /// <summary>
        /// 自动根据类型生成template名字
        /// </summary>
        public string TemplateName => $"{CodeStringConfig.TitleString.AsTemplatesObjectTitle}{Unique_ID}";
    }
}
