using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.Data.String.Messages
{
    /// <summary>
    /// 保存了一些UI的字符
    /// </summary>
    public static class AsHabitateString
    {
        /// <summary>
        /// 私有的标记函数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string _(string input) => input;

        /// <summary>
        /// 错误字符
        /// </summary>
        public static class Error
        {
            /// <summary>
            /// 太空舱描述信息丢失了! 这很可能是个bug, 可以向我反馈一下
            /// </summary>
            public static AsString HabitatDiscribeLost = _("The habitat information is missing! This is probably a bug. You can give me feedback");

            /// <summary>
            /// 太空舱描述信息丢了一行......这很可能是个bug, 可以向我反馈一下
            /// </summary>
            public static AsString HabitatDiscribeLostALine = _("A line is missing from the habitat description This is probably a bug. You can give me feedback");

            /// <summary>
            /// 需要1个液体输出口，但是描述里有{0}个，在问题修正之前，不能进行保存
            /// </summary>
            public static AsString RocketInteriorLiquidOutputPortNumberError = _("1 liquid output port is required, but there are {0} in the description, which cannot be saved until the problem is corrected");

            /// <summary>
            /// 需要1个液体输入口，但是描述里有{0}个，在问题修正之前，不能进行保存
            /// </summary>
            public static AsString RocketInteriorLiquidInputPortNumberError = _("1 liquid input port is required, but there are {0} in the description, which cannot be saved until the problem is corrected");

            /// <summary>
            /// 需要1个气体输出口，但是描述里有{0}个，在问题修正之前，不能进行保存
            /// </summary>
            public static AsString RocketInteriorGasOutputPortNumberError = _("1 gas output port is required, but there are {0} in the description, which cannot be saved until the problem is corrected");

            /// <summary>
            /// 需要1个气体输入口，但是描述里有{0}个，在问题修正之前，不能进行保存
            /// </summary>
            public static AsString RocketInteriorGasInputPortNumberError = _("1 gas input port is required, but there are {0} in the description, which cannot be saved until the problem is corrected");

            /// <summary>
            /// 需要1个门，但是描述里有{0}个，在问题修正之前，不能进行保存
            /// </summary>
            public static AsString ClustercraftInteriorDoorNumberError = _("1 door is required, but there are {0} in the description. It cannot be saved until the problem is corrected");

            /// <summary>
            /// 描述信息说太空舱的高度小于4格，在这么小的高度下没有办法生成能正常使用的太空舱，这很可能是个bug, 可以向我反馈一下
            /// </summary>
            public static AsString HabitatHeightError = _("The description information says that the height of the habitat is less than 4 grids. At such a small height, there is no way to generate a habitat that can be used normally. ");

            /// <summary>
            /// 太空舱门不能在最高的那一层，这样的话就没法生成太空舱了
            /// </summary>
            public static AsString HabitatInitError = _("The habitat door cannot be on the highest floor, so it cannot be generated");

            /// <summary>
            /// 太空舱门入口受阻，看看是不是被堵住了
            /// </summary>
            public static AsString HabitatPortalError = _("The entrance of the habitat door is blocked, see if it is blocked");
        }

        /// <summary>
        /// 操作完成显示的字符
        /// </summary>
        public static class Success
        {
            /// <summary>
            /// 太空舱描述信息保存成功
            /// </summary>
            public static AsString HabitatSaveSuccess = _("The habitat information saved successfully");

            /// <summary>
            /// 太空舱描述信息加载成功
            /// </summary>
            public static AsString HabitatLoadSuccess = _("The habitat information loaded successfully");

            /// <summary>
            /// 太空舱描述信息默认设置覆盖成功
            /// </summary>
            public static AsString HabitatOverwrittenSuccess = _("The habitat description information is successfully overwritten by default");
        }

        /// <summary>
        /// 配置文件的字符
        /// </summary>
        public static class BuildingConfig
        {
            /// <summary>
            /// 星舰
            /// </summary>
            public static AsString Name = _("Starship");

            /// <summary>
            /// 花费巨大
            /// </summary>
            public static AsString Description = _("cost huge");

            /// <summary>
            /// 很大
            /// </summary>
            public static AsString Effect = _("huge");
        }

        /// <summary>
        /// 一些专用于UI组件的信息
        /// </summary>
        public static class UI
        {
            /// <summary>
            /// 一些专用于按钮的信息
            /// </summary>
            public static class Button
            {
                /// <summary>
                /// 设计太空舱
                /// </summary>
                public static readonly AsString DesignButton = _("Design habitate");

                /// <summary>
                /// 点击此处设计并实时生成太空舱
                /// </summary>
                public static readonly AsString DesignButtonTooltipText = _("Click here to design the habitate and generate it in real time");
            }
        }
    }
}
