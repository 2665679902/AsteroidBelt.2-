using AsteroidBelt.Data.templates.Building.Rocket.Habitat;
using AsteroidBelt.Data.templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using static Grid.Restriction;
using AsTool.Assert;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.ELEMENTS;
using YamlDotNet.Serialization;
using UnityEngine;
using static STRINGS.UI.SANDBOXTOOLS.SETTINGS;

namespace AsteroidBelt.DataBuilder
{
    /// <summary>
    /// 一个从<see cref="AsTemplatesObject"/>构建具体容器信息的工具类
    /// </summary>
    public static class AsTemplatesObjectBuilder
    {
        /// <summary>
        /// 构建火箭仓模板容器的工具类
        /// </summary>
        private static class HugeHabitatDataBuildUtility
        {
            /// <summary>
            /// 获取对应容器
            /// </summary>
            /// <param name="habitatData">要构建的信息</param>
            /// <returns>生成结果</returns>
            public static TemplateContainer GetContainer(HugeHabitatData habitatData)
            {
                //生成前判断状态
                AsAssert.IsTrue(habitatData.Check(out var errorString), errorString);

                List<Cell> cellList = new List<Cell>();

                List<Prefab> buildingList = new List<Prefab>();

                List<Prefab> otherEntitiesList = new List<Prefab>();

                int habitatHeight = habitatData.HabitatDiscribe.Count;

                int habitatLength = habitatData.HabitatDiscribe[0].Count;

                int middle = habitatLength / 2;

                //设置除了控制台的其他东西的状态(暂时先不放控制台了)
                for (int loc_y = 0; loc_y < habitatHeight; loc_y++)
                {
                    for (int loc_x = 0; loc_x < habitatLength; loc_x++)
                    {
                        Vector2I GetCurrentLoc()
                        {
                            return new Vector2I(loc_x - middle, middle - loc_y -1);
                        }

                        var num = habitatData.HabitatDiscribe[loc_y][loc_x];

                        switch (num)
                        {
                            case HugeHabitatData.Nothing:
                                break;

                            case HugeHabitatData.Vacuum:
                                cellList.Add(Clone(DefaultVacuumCell, GetCurrentLoc()));
                                break;

                            case HugeHabitatData.RocketEnvelopeWindowTile:
                                cellList.Add(Clone(DefaultDiamondCell, GetCurrentLoc()));
                                buildingList.Add(DefaultRocketEnvelopeWindowTile.Clone(GetCurrentLoc()));
                                break;

                            case HugeHabitatData.RocketWallTile:
                                cellList.Add(Clone(DefaultSteelCell, GetCurrentLoc()));
                                buildingList.Add(DefaultRocketWallTile.Clone(GetCurrentLoc()));
                                break;

                            case HugeHabitatData.RocketInteriorLiquidOutputPort:
                                cellList.Add(Clone(DefaultSteelCell, GetCurrentLoc()));
                                buildingList.Add(DefaultRocketInteriorLiquidOutputPort.Clone(GetCurrentLoc()));
                                break;

                            case HugeHabitatData.RocketInteriorLiquidInputPort:
                                cellList.Add(Clone(DefaultSteelCell, GetCurrentLoc()));
                                buildingList.Add(DefaultRocketInteriorLiquidInputPort.Clone(GetCurrentLoc()));
                                break;

                            case HugeHabitatData.RocketInteriorGasOutputPort:
                                cellList.Add(Clone(DefaultSteelCell, GetCurrentLoc()));
                                buildingList.Add(DefaultRocketInteriorGasOutputPort.Clone(GetCurrentLoc()));
                                break;

                            case HugeHabitatData.RocketInteriorGasInputPort:
                                cellList.Add(Clone(DefaultSteelCell, GetCurrentLoc()));
                                buildingList.Add(DefaultRocketInteriorGasInputPort.Clone(GetCurrentLoc()));
                                break;

                            case HugeHabitatData.ClustercraftInteriorDoor:

                                cellList.Add(Clone(DefaultSteelCell, GetCurrentLoc()));
                                buildingList.Add(DefaultRocketWallTile.Clone(GetCurrentLoc()));

                                //获取门上面那个格子, 格子的属性也设置成铁(如果是空的话)
                                var cellAbove = cellList.Find((cell) => { var loc = GetCurrentLoc(); return cell.location_x == loc.x && cell.location_y == loc.y; });

                                if (cellAbove != null)
                                {
                                    int cellaboveIndex = cellList.IndexOf(cellAbove);

                                    if (cellList[cellaboveIndex].element == SimHashes.Vacuum)
                                    {
                                        cellList[cellaboveIndex] = Clone(DefaultSteelCell, new Vector2I(cellAbove.location_x, cellAbove.location_y));

                                        //获取门上面那个格子, 格子的物体也设置成火箭墙
                                        var prefabAbove = buildingList.Find((prefab) => { var loc = GetCurrentLoc(); return prefab.location_x == loc.x && prefab.location_y == loc.y; });

                                        if (prefabAbove != null)
                                        {
                                            buildingList[buildingList.IndexOf(prefabAbove)] = DefaultRocketWallTile.Clone(new Vector2I(prefabAbove.location_x, prefabAbove.location_y));
                                        }
                                    }

                                }

                                //设置门
                                otherEntitiesList.Add(DefaultClustercraftInteriorDoor.Clone(GetCurrentLoc()));

                                break;

                        }
                    }

                }

                var result = new TemplateContainer
                {
                    name = habitatData.TemplateName
                };

                result.Init(cellList, buildingList, null, null, otherEntitiesList);

                return result;
            }

            /// <summary>
            /// 克隆格子到一个相对位置
            /// </summary>
            /// <param name="cell">格子</param>
            /// <param name="vector">相对位置</param>
            /// <returns>克隆结果</returns>
            private static Cell Clone(Cell cell, Vector2I vector)
            {
                return new Cell
                {
                    location_x = cell.location_x + vector.x,
                    location_y = cell.location_y + vector.y,
                    element = cell.element,
                    temperature = cell.temperature,
                    mass = cell.mass,
                    diseaseName = cell.diseaseName,
                    diseaseCount = cell.diseaseCount,
                    preventFoWReveal = cell.preventFoWReveal,
                };
            }

            /// <summary>
            /// 默认的墙描述
            /// </summary>
            private static readonly Prefab DefaultRocketWallTile =
               new Prefab(
                   _id: "RocketWallTile", _type: Prefab.Type.Other, loc_x: 0, loc_y: 0, _element: SimHashes.Steel, _temperature: 293.149994f, _units: 0,
                   _disease: null, _disease_count: 0, _rotation: Orientation.Neutral, _amount_values: null, _other_values: null, _connections: 0);

            /// <summary>
            /// 默认的玻璃窗描述
            /// </summary>
            private static readonly Prefab DefaultRocketEnvelopeWindowTile =
                new Prefab(
                    _id: "RocketEnvelopeWindowTile", _type: Prefab.Type.Other, loc_x: 0, loc_y: 0, _element: SimHashes.Diamond, _temperature: 293.149994f, _units: 0,
                    _disease: null, _disease_count: 0, _rotation: Orientation.Neutral, _amount_values: null, _other_values: null, _connections: 0);

            /// <summary>
            /// 默认的液体输出端口描述
            /// </summary>
            private static readonly Prefab DefaultRocketInteriorLiquidOutputPort =
               new Prefab(
                   _id: "RocketInteriorLiquidOutputPort", _type: Prefab.Type.Other, loc_x: 0, loc_y: 0, _element: SimHashes.Steel, _temperature: 293.149994f, _units: 0,
                   _disease: null, _disease_count: 0, _rotation: Orientation.Neutral, _amount_values: null, _other_values: null, _connections: 0);

            /// <summary>
            /// 默认的液体输入端口描述
            /// </summary>
            private static readonly Prefab DefaultRocketInteriorLiquidInputPort =
                new Prefab(
                    _id: "RocketInteriorLiquidInputPort", _type: Prefab.Type.Other, loc_x: 0, loc_y: 0, _element: SimHashes.Steel, _temperature: 293.149994f, _units: 0,
                    _disease: null, _disease_count: 0, _rotation: Orientation.Neutral, _amount_values: null, _other_values: null, _connections: 0);

            /// <summary>
            /// 默认的气体输出端口描述
            /// </summary>
            private static readonly Prefab DefaultRocketInteriorGasOutputPort =
               new Prefab(
                   _id: "RocketInteriorGasOutputPort", _type: Prefab.Type.Other, loc_x: 0, loc_y: 0, _element: SimHashes.Steel, _temperature: 293.149994f, _units: 0,
                   _disease: null, _disease_count: 0, _rotation: Orientation.Neutral, _amount_values: null, _other_values: null, _connections: 0);

            /// <summary>
            /// 默认的气体输入端口描述
            /// </summary>
            private static readonly Prefab DefaultRocketInteriorGasInputPort =
                new Prefab(
                    _id: "RocketInteriorGasInputPort", _type: Prefab.Type.Other, loc_x: 0, loc_y: 0, _element: SimHashes.Steel, _temperature: 293.149994f, _units: 0,
                    _disease: null, _disease_count: 0, _rotation: Orientation.Neutral, _amount_values: null, _other_values: null, _connections: 0);

            /// <summary>
            /// 默认的火箭控制台
            /// </summary>
            private static readonly Prefab DefaultRocketControlStation =
                new Prefab(
                    _id: "RocketControlStation", _type: Prefab.Type.Other, loc_x: 0, loc_y: 0, _element: SimHashes.Cuprite, _temperature: 293.149994f, _units: 0,
                    _disease: null, _disease_count: 0, _rotation: Orientation.Neutral, _amount_values: null, _other_values: null, _connections: 0);

            /// <summary>
            /// 默认的火箭内部的门
            /// </summary>
            private static readonly Prefab DefaultClustercraftInteriorDoor =
                new Prefab(
                    _id: "ClustercraftInteriorDoor", _type: Prefab.Type.Other, loc_x: 0, loc_y: 0, _element: SimHashes.Unobtanium, _temperature: 293.149994f, _units: 1,
                    _disease: null, _disease_count: 0, _rotation: Orientation.Neutral, _amount_values: null, _other_values: null, _connections: 0);

            /// <summary>
            /// 默认的铁元素格子
            /// </summary>
            private static readonly Cell DefaultSteelCell =
                new Cell(loc_x: 0, loc_y: 0, _element: SimHashes.Steel, _temperature: 293.149994f, _mass: 100, _diseaseName: null, _diseaseCount: 0, _preventFoWReveal: false);

            /// <summary>
            /// 默认的真空元素格子
            /// </summary>
            private static readonly Cell DefaultVacuumCell =
                new Cell(loc_x: 0, loc_y: 0, _element: SimHashes.Vacuum, _temperature: 0f, _mass: 0, _diseaseName: null, _diseaseCount: 0, _preventFoWReveal: false);

            /// <summary>
            /// 默认的钻石元素格子
            /// </summary>
            private static readonly Cell DefaultDiamondCell =
                new Cell(loc_x: 0, loc_y: 0, _element: SimHashes.Diamond, _temperature: 293.149994f, _mass: 100, _diseaseName: null, _diseaseCount: 0, _preventFoWReveal: false);
        }

        /// <summary>
        /// 从<see cref="HugeHabitatData"/>构建具体容器信息的工具类
        /// </summary>
        /// <param name="habitatData">需要构建的数据</param>
        /// <returns>构建结果</returns>
        public static TemplateContainer GetContainer(this HugeHabitatData habitatData)
        {
            return HugeHabitatDataBuildUtility.GetContainer(habitatData);
        }
    }
}
