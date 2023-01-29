using AsteroidBelt.Data.String.Messages;
using AsTool.Assert;
using AsTool.Common.Extension;
using AsTool.Reflection.AsSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidBelt.Data.templates.Building.Rocket.Habitat
{
    /// <summary>
    /// 一个用于保存星舰状态的数据类型
    /// </summary>
    public class HugeHabitatData: AsTemplatesObject
    {
        #region 设置

        /// <summary>
        /// 什么都没有(宇宙背景)
        /// </summary>
        public const int Nothing = 0;

        /// <summary>
        /// 真空
        /// </summary>
        public const int Vacuum = 1;

        /// <summary>
        /// 玻璃墙
        /// </summary>
        public const int RocketEnvelopeWindowTile = 2;

        /// <summary>
        /// 火箭墙
        /// </summary>
        public const int RocketWallTile = 3;

        /// <summary>
        /// 液体输出端口
        /// </summary>
        public const int RocketInteriorLiquidOutputPort = 4;

        /// <summary>
        /// 液体输入端口
        /// </summary>
        public const int RocketInteriorLiquidInputPort = 5;

        /// <summary>
        /// 气体输出端口
        /// </summary>
        public const int RocketInteriorGasOutputPort = 6;

        /// <summary>
        /// 气体输入端口
        /// </summary>
        public const int RocketInteriorGasInputPort = 7;

        /// <summary>
        /// 火箭内的门
        /// </summary>
        public const int ClustercraftInteriorDoor = 8;

        #endregion


        /// <summary>
        /// 储存对于火箭仓内部的描述
        /// </summary>
        public List<List<int>> HabitatDiscribe { get; set; }

        /// <summary>
        /// 获取单一列表形式的描述信息, 该消息隐藏了行列关系
        /// </summary>
        [AsNoSerialize]
        public List<int> HabitatDiscribeInList {
            get
            {
                if(HabitatDiscribe is null)
                    return null;

                var totalList = new List<int>();

                foreach (var item in HabitatDiscribe)
                {
                    totalList.AddRange(item);
                }

                return totalList;
            } 
        }

        /// <summary>
        /// 检查当前描述信息
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>如果出现错误返回假, 否则返回真</returns>
        public bool Check(out string errorMessage)
        {
            if(HabitatDiscribe is null)
            {
                errorMessage = AsHabitateString.Error.HabitatDiscribeLost;

                return false;
            }

            if(HabitatDiscribe.Any(list => list is null))
            {
                errorMessage = AsHabitateString.Error.HabitatDiscribeLostALine;

                return false;
            }

            var totalList = HabitatDiscribeInList;

            if (totalList.AsCount(RocketInteriorLiquidOutputPort) != 1)
            {
                errorMessage = AsHabitateString.Error.RocketInteriorLiquidOutputPortNumberError.Translate(totalList.AsCount(RocketInteriorLiquidOutputPort));

                return false;
            }

            if (totalList.AsCount(RocketInteriorLiquidInputPort) != 1)
            {
                errorMessage = AsHabitateString.Error.RocketInteriorLiquidInputPortNumberError.Translate(totalList.AsCount(RocketInteriorLiquidInputPort)); 

                return false;
            }

            if (totalList.AsCount(RocketInteriorGasOutputPort) != 1)
            {
                errorMessage = AsHabitateString.Error.RocketInteriorGasOutputPortNumberError.Translate(totalList.AsCount(RocketInteriorGasOutputPort)); 

                return false;
            }

            if (totalList.AsCount(RocketInteriorGasInputPort) != 1)
            {
                errorMessage = AsHabitateString.Error.RocketInteriorGasInputPortNumberError.Translate(totalList.AsCount(RocketInteriorGasInputPort));

                return false;
            }

            if (totalList.AsCount(ClustercraftInteriorDoor) != 1)
            {
                errorMessage = AsHabitateString.Error.ClustercraftInteriorDoorNumberError.Translate(totalList.AsCount(ClustercraftInteriorDoor));

                return false;
            }

            //补齐数列, 将列表补齐到矩形

            int maxCount = 0;

            foreach (var item in HabitatDiscribe)
            {
                maxCount = Math.Max(maxCount, item.Count);
            }

            foreach(var item in HabitatDiscribe)
            {
                while(item.Count < maxCount)
                {
                    item.Add(0);
                }
            }

            //判断传送门位置是否合法

            if (HabitatDiscribe.Count < 4)
            {
                errorMessage = AsHabitateString.Error.HabitatHeightError;

                return false;
            }

            foreach(var item in HabitatDiscribe)
            {
                if (item.Contains(ClustercraftInteriorDoor))
                {
                    var y = HabitatDiscribe.IndexOf(item);

                    var x = item.IndexOf(ClustercraftInteriorDoor);

                    if(y == 0)
                    {
                        errorMessage = AsHabitateString.Error.HabitatInitError;

                        return false;
                    }
                    else if (
                        (x + 1 < item.Count && item[x + 1] != Vacuum && item[x + 1] != Nothing)||
                        (y - 1 < HabitatDiscribe.Count && HabitatDiscribe[y - 1][x + 1] != Vacuum && HabitatDiscribe[y - 1][x + 1] != Nothing))
                    {
                        errorMessage = AsHabitateString.Error.HabitatPortalError;

                        return false;
                    }

                }
            }

            errorMessage = "";

            return true;
        }
    }
}
