using AsteroidBelt.Data.String;
using AsteroidBelt.HookUtility.StringUtility;
using AsTool.Assert;
using AsTool.Load;
using AsTool.Reflection;
using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.HookUtility.BuildingUtility
{
    /// <summary>
    /// 添加建筑的工具
    /// </summary>
    public static class BuildingInjecter
    {
        /// <summary>
        /// 加载时注册加载事件
        /// </summary>
        [AsLoad]
        internal static void Load()
        {
            //AsLog.Debug("建筑注入加载");

            foreach (var type in AsType.GetAnyAsTypesFrom(AsLoadManager.AssemblieLoaded, new Type[] { typeof(AsBuildingConfig) }))
            {
                if (type.CompelInit() is AsBuildingConfig item && item != null)
                {
                    void InjectString()
                    {
                        //1
                        String.AddBuildingStrings(item.ID, item.Name, item.Description, item.Effect);
                    }

                    void InjectTech()
                    {
                        //2
                        Tech.AddBuildingToTech(item.ID, item.Level);
                    }

                    StringInjecter.LocalizationFinished += InjectString;
                    Tech.TechsInit += InjectTech;

                    if (item.AddToRocketScreen)
                    {
                        Screen.AddBuildingToRocketScreen(item.ID);
                    }
                    else
                    {
                        Screen.AddBuildingToPlanScreen(item.ScreenCategory, item.ID, item.SubcategoryID);
                    }
                }
            }
        }

        /// <summary>
        /// 简单建筑的复合配置文件
        /// </summary>
        public abstract class AsBuildingConfig: IBuildingConfig
        {
            /// <summary>
            /// 默认仅DLC1可用
            /// </summary>
            /// <returns>返回对应字符</returns>
            public override string[] GetDlcIds()
            {
                return DlcManager.AVAILABLE_EXPANSION1_ONLY;
            }

            /// <summary>
            /// 必须在派生类中显示声明ID
            /// </summary>
            public abstract string ID { get; }

            /// <summary>
            /// 生成时指定Tech等级
            /// </summary>
            public abstract Tech.TechLevels Level { get; }

            /// <summary>
            /// 注册在建筑面板的类型
            /// </summary>
            public abstract Screen.PlanScreenCategory ScreenCategory { get; }

            /// <summary>
            /// 面板的次级种类的标签(不要也可以)
            /// </summary>
            public virtual string SubcategoryID { get => "uncategorized"; }

            /// <summary>
            /// 是否注册到火箭建造面板, 默认不注册
            /// </summary>
            public virtual bool AddToRocketScreen { get => false; }

            /// <summary>
            /// 生成时指定名字
            /// </summary>
            public abstract AsString Name { get; }

            /// <summary>
            /// 生成时指定描述
            /// </summary>
            public abstract AsString Description { get; }

            /// <summary>
            /// 生成时指定影响
            /// </summary>
            public abstract AsString Effect { get; }
        }

        /// <summary>
        /// 介入科技的工具
        /// </summary>
        public static class Tech
        {
            /// <summary>
            /// 截获 Techs 生成事件
            /// </summary>
            [HarmonyPatch(typeof(Techs), "Init")]
            private static class Techs_Patch
            {
                public static void Postfix(ref Techs __instance)
                {
                    TechsInstance = __instance;

                    TechsInited = true;

                    AsLog.Info("科技管理类初始化 Techs Init");

                    TechsInit?.Invoke();
                }
            }

            /// <summary>
            /// Tech 的具体等级
            /// </summary>
            public enum TechLevels
            {
                /// <summary>
                /// 基础耕作的解锁将允许玩家使用最基础的农业设施种植箱进行人工种植。并使用藻类箱进行简单的二氧化碳处理。此外，他还允许建造口粮箱;允许使用堆肥堆处理污染土污染土或其他可堆肥物。
                /// </summary>
                FarmingTech,

                /// <summary>
                /// 食物制备允许使用电动烤炉中的食谱来制作食物，并用餐桌为复制人提供吃饭的地方。它还解锁了土培砖，这是种植箱的升级版。
                /// </summary>
                FineDining,

                /// <summary>
                /// 食物再利用 -> 香料研磨器和榨汁机
                /// </summary>
                FoodRepurposing,

                /// <summary>
                /// 美食制备 -> 天然气灶
                /// </summary>
                FinerDining,

                /// <summary>
                /// 农业解锁了第三个用于种植的建筑液培砖，以及可以生产微肥微肥和少量天然气天然气的肥料合成器。此外还解锁了冰箱，这是一个消耗电力以减缓内部食物腐烂速度的建筑物。在开启了眼冒金星图标的情况下，农业还会解锁辐射灯。
                /// </summary>
                Agriculture,

                /// <summary>
                /// 动物养殖可以让你驯服和饲养小动物，让它们产下更多的蛋，并（在某些情况下）生产更多的材料。解锁的修剪站可以为毛鳞壁虎和滑鳞壁虎剪毛来获得相应的材料。同时还解锁了飞行动物诱饵用于一次性捕捉飞行生物。
                /// </summary>
                Ranching,

                /// <summary>
                /// 动物控制研究解锁了可以快速孵化小动物的孵化器、可以用于实现自动化控制的小动物传感器以及用于捕获小动物的两种陷阱。
                /// </summary>
                AnimalControl,

                /// <summary>
                /// 空气系统技术解锁了两种高效的制氧建筑：电解器和铁锈脱氧机。
                /// </summary>
                ImprovedOxygen,

                /// <summary>
                /// 通风技术解锁了气体管道系统的基础要素，使玩家能够简单地抽送气体
                /// </summary>
                GasPiping,

                /// <summary>
                /// 改良通风 -> 隔热气体管道 气压传感器 气体开关阀 高压排气口
                /// </summary>
                ImprovedGasPiping,

                /// <summary>
                /// 改良气体流动技术解锁了火箭装卸气体的能力，同时解锁了简单易用的二氧化碳引擎。
                /// </summary>
                SpaceGas,

                /// <summary>
                /// 压强管理技术可以解锁两种控制流体流量的调节阀、能够透过气体而阻挡液体的透气砖，以及可以阻挡气液体通过的手动气闸。在眼冒金星图标中，该科技是改良气体流动科技的前置，而后者能解锁一些气体相关的火箭技术。
                /// </summary>
                PressureManagement,

                /// <summary>
                /// 除污技术解锁了将污染氧净化为氧气的空气净化器、用水清理二氧化碳的碳素脱离器，和用于自动控制空气流动的机械气闸。
                /// </summary>
                DirectedAirStreams,

                /// <summary>
                /// 过滤技术提供了分离一些混杂物质的技术——比如分离混合流体的气体过滤器和液体过滤器，以及眼冒金星图标中分离水和泥土的泥浆分离器。
                /// </summary>
                LiquidFiltering,

                /// <summary>
                /// 药物学 -> 配药桌
                /// </summary>
                MedicineI,

                /// <summary>
                /// 
                /// </summary>
                MedicineII,

                /// <summary>
                /// 
                /// </summary>
                MedicineIII,

                /// <summary>
                /// 
                /// </summary>
                MedicineIV,

                /// <summary>
                /// 
                /// </summary>
                LiquidPiping,

                /// <summary>
                /// 
                /// </summary>
                ImprovedLiquidPiping,

                /// <summary>
                /// 
                /// </summary>
                PrecisionPlumbing,

                /// <summary>
                /// 
                /// </summary>
                SanitationSciences,

                /// <summary>
                /// 
                /// </summary>
                FlowRedirection,

                LiquidDistribution,

                AdvancedSanitation,

                AdvancedFiltration,

                Distillation,

                Catalytics,

                AdvancedResourceExtraction,

                PowerRegulation,

                AdvancedPowerRegulation,

                PrettyGoodConductors,

                RenewableEnergy,

                Combustion,

                ImprovedCombustion,

                InteriorDecor,

                Artistry,

                Clothing,

                Acoustics,

                SpacePower,

                NuclearRefinement,

                FineArt,

                EnvironmentalAppreciation,

                Luxury,

                RefractiveDecor,

                GlassFurnishings,

                Screens,

                RenaissanceArt,

                Plastics,

                ValveMiniaturization,

                HydrocarbonPropulsion,

                BetterHydroCarbonPropulsion,

                CryoFuelPropulsion,

                Suits,

                Jobs,

                AdvancedResearch,

                SpaceProgram,

                CrashPlan,

                DurableLifeSupport,

                NuclearResearch,

                AdvancedNuclearResearch,

                NuclearStorage,

                NuclearPropulsion,

                NotificationSystems,

                ArtificialFriends,

                BasicRefinement,

                RefinedObjects,

                Smelting,

                HighTempForging,

                HighPressureForging,

                RadiationProtection,

                TemperatureModulation,

                HVAC,

                LiquidTemperature,

                LogicControl,

                GenericSensors,

                LogicCircuits,

                ParallelAutomation,

                DupeTrafficControl,

                Multiplexing,

                SkyDetectors,

                TravelTubes,

                SmartStorage,

                SolidManagement,

                HighVelocityTransport,

                BasicRocketry,

                CargoI,

                CargoII,

                CargoIII,

                EnginesI,

                EnginesII,

                EnginesIII,

                Jetpacks,

                SolidTransport,

                Monuments,

                SolidSpace,

                RoboticTools,

                PortableGasses,

                Bioengineering,

                SpaceCombustion,

                HighVelocityDestruction,

                GasDistribution,

                AdvancedScanners
            }

            /// <summary>
            /// <see cref="Techs"/>实例，仅当初始化完成时可使用
            /// </summary>
            public static Techs TechsInstance { get; private set; }

            /// <summary>
            /// 用于获取科技信息实例是否被激活
            /// </summary>
            public static bool TechsInited { get; private set; }

            /// <summary>
            /// 当科技信息创建完成时激活此事件, 提示可使用信息注册行为
            /// </summary>
            public static event System.Action TechsInit;

            /// <summary>
            /// 尝试添加一个建筑信息到某个科技信息
            /// </summary>
            /// <param name="buildingID">建筑的id</param>
            /// <param name="techID">科技的id</param>
            /// <returns>是否添加成功</returns>
            public static bool AddBuildingToTech(string buildingID, TechLevels techID)
            {
                return AddBuildingToTech(buildingID, techID.ToString());
            }

            /// <summary>
            /// 尝试添加一个建筑信息到某个科技信息
            /// </summary>
            /// <param name="buildingID">建筑的id</param>
            /// <param name="techID">科技的id</param>
            /// <returns>是否添加成功</returns>
            public static bool AddBuildingToTech(string buildingID, string techID)
            {
                AsAssert.NotNull(buildingID, "buildingID lost");
                AsAssert.NotNull(techID, "techID lost");

                //如果科技记录尚未生成, 直接返回
                if (!TechsInited)
                {
                    AsLog.Error($"{buildingID} try to add building to Tech before Techs generated");
                    return false;
                }

                var targetTech = TechsInstance.resources.First(tech => tech.Id == techID);// id 和 name 在这里好像是一样的

                if(targetTech is null )
                {
                    AsLog.Error($"can not find techID: {techID}");
                    return false;
                }

                //AsLog.Debug($"Add building {buildingID} to {targetTech.Id}");

                targetTech.AddUnlockedItemIDs(buildingID);

                return true;
            }
        }

        /// <summary>
        /// 用于将建筑添加到面板
        /// </summary>
        public static class Screen
        {
            /// <summary>
            /// 所有面板的种类
            /// </summary>
            public enum PlanScreenCategory
            {
                /// <summary>
                /// 此等级表示不会将建筑注入任何一个建筑面板
                /// </summary>
                None,

                Base,

                Oxygen,

                Power,

                Food,

                Plumbing,

                HVAC,

                Refining,

                Medical,

                Furniture,

                Equipment,

                Utilities,

                Automation,

                Conveyance,

                Rocketry,

                HEP
            }

            /// <summary>
            /// 将建筑添加到某一建筑面板
            /// </summary>
            /// <param name="category">面板类别</param>
            /// <param name="buildingID">建筑的id</param>
            /// <param name="subcategoryID">次级类别的名称</param>
            public static void AddBuildingToPlanScreen(PlanScreenCategory category, string buildingID,string subcategoryID)
            {
                AsAssert.NotNull(buildingID, "buildingID lost");

                if (category == PlanScreenCategory.None)
                    return;

                ModUtil.AddBuildingToPlanScreen(new HashedString(category.ToString()), buildingID, subcategoryID); 
            }

            /// <summary>
            /// 将建筑添加到火箭面板, 如果本来就有就不会再添加了
            /// </summary>
            /// <param name="buildingID">建筑的id</param>
            /// <param name="index">添加的位置, 如果超出范围则会添加在最后</param>
            public static void AddBuildingToRocketScreen(string buildingID, int index = 999)
            {
                AsAssert.NotNull(buildingID, "buildingID lost");
                AsAssert.IsTrue(index > 0, $"get an unexcept index {index} which < 0");

                if (SelectModuleSideScreen.moduleButtonSortOrder.Contains(buildingID))
                {
                    return;
                }

                index = Math.Min(index, SelectModuleSideScreen.moduleButtonSortOrder.Count);
                SelectModuleSideScreen.moduleButtonSortOrder.Insert(index, buildingID);
            }
        }

        /// <summary>
        /// 添加建筑文字的工具
        /// </summary>
        public static class String
        {
            /// <summary>
            /// 添加一个建筑文字
            /// </summary>
            /// <param name="buildingId">建筑的id</param>
            /// <param name="name">建筑的名字</param>
            /// <param name="description">建筑的描述</param>
            /// <param name="effect">建筑的影响</param>
            public static void AddBuildingStrings(string buildingId, string name, string description, string effect)
            {
                Strings.Add(new string[]
                {
                "STRINGS.BUILDINGS.PREFABS." + buildingId.ToUpperInvariant() + ".NAME",
                STRINGS.UI.FormatAsLink(name, buildingId)
                });
                Strings.Add(new string[]
                {
                "STRINGS.BUILDINGS.PREFABS." + buildingId.ToUpperInvariant() + ".DESC",
                description
                });
                Strings.Add(new string[]
                {
                "STRINGS.BUILDINGS.PREFABS." + buildingId.ToUpperInvariant() + ".EFFECT",
                effect
                });
            }
        }

    }
}
