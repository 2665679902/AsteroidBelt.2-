using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.Data.String
{
    /// <summary>
    /// 记录了一些代码会用到的标记用的字符串
    /// </summary>
    public static class CodeStringConfig
    {
        /// <summary>
        /// 记录了一些用于组合成标识符头的标准字符
        /// </summary>
        public static class TitleString
        {
            /// <summary>
            /// 数据类型ID的默认头
            /// </summary>
            public static string AsDataObjectTitle = "DataObject_";

            /// <summary>
            /// 模板类型ID的默认头
            /// </summary>
            public static string AsTemplatesObjectTitle = "expansion1::asmod_fake_folder/";
        }

        /// <summary>
        /// UI使用的字符
        /// </summary>
        public static class UIString
        {
            /// <summary>
            /// 面板的名字
            /// </summary>
            public static string CanvasName = "ModUICanvas";

            /// <summary>
            /// 面板组的名字
            /// </summary>
            public static class PanelGroupName
            {
                /// <summary>
                /// 根面板组的名字
                /// </summary>
                public static string RootLevel = "ModConfig_Root";
            }
        }
    }
}
