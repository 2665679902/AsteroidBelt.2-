using AsteroidBelt.Data.String;
using AsteroidBelt.Data.String.Messages;
using AsteroidBelt.Data.templates.Building.Rocket.Habitat;
using AsteroidBelt.HookUtility.BuildingUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AsteroidBelt.Config.ONIConfig.BuildingConfig
{
    internal class HugeHabitatConfig : BuildingInjecter.AsBuildingConfig
    {
        /// <summary>
        /// 私有用于读取数据的实体
        /// </summary>
        private readonly HugeHabitatData habitatData = new HugeHabitatData();

        public override string ID => habitatData.TemplateName;

        public override BuildingInjecter.Tech.TechLevels Level => BuildingInjecter.Tech.TechLevels.DurableLifeSupport;

        public override BuildingInjecter.Screen.PlanScreenCategory ScreenCategory => BuildingInjecter.Screen.PlanScreenCategory.None;

        public override bool AddToRocketScreen => true;

        public override AsString Name => AsHabitateString.BuildingConfig.Name;

        public override AsString Description => AsHabitateString.BuildingConfig.Description;

        public override AsString Effect => AsHabitateString.BuildingConfig.Effect;

        /// <summary>
        /// 生成一个建筑设置
        /// </summary>
        /// <returns>返回太空舱建筑设置</returns>
        public override BuildingDef CreateBuildingDef()
        {
            //ID 使用数据的 TemplateName
            string id = ID;

            //宽度是5
            int width = 5;

            //高度是4
            int height = 4;

            //动画使用火箭仓方案
            string anim = "rocket_habitat_medium_module_kanim";

            //?
            int hitpoints = 1000;

            //建筑事件: 60秒
            float construction_time = 60f;

            //质量 -> 2000f
            float[] dense_TIER = BUILDINGS.ROCKETRY_MASS_KG.DENSE_TIER3;

            //建筑材料 -> 金属
            string[] raw_METALS = MATERIALS.RAW_METALS;

            //熔点 -> 9999K
            float melting_point = 9999f;

            //允许在任何地方建造
            BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;

            //设置噪音
            EffectorValues tier = NOISE_POLLUTION.NOISY.TIER2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, dense_TIER, raw_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, tier, 0.2f);
            
            //配置为火箭建筑文件
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);

            //使用火箭tag
            buildingDef.AttachmentSlotTag = GameTags.Rocket;

            //将火箭放在建筑层
            buildingDef.SceneLayer = Grid.SceneLayer.Building;

            //过热温度 2273.15
            buildingDef.OverheatTemperature = 2273.15f;

            //不可淹没
            buildingDef.Floodable = false;

            //物体层级, 放在建筑层
            buildingDef.ObjectLayer = ObjectLayer.Building;

            //在格子的前面
            buildingDef.ForegroundLayer = Grid.SceneLayer.Front;

            //不需要电力输入
            buildingDef.RequiresPowerInput = false;

            //可交互的格子位置 -> 0,0
            buildingDef.attachablePosition = new CellOffset(0, 0);

            //建筑可移动
            buildingDef.CanMove = true;

            //建筑可取消
            buildingDef.Cancellable = false;

            //建筑不在建筑菜单里面显示
            buildingDef.ShowInBuildMenu = false;


            return buildingDef;
        }

        /// <summary>
        /// 配置构建模板
        /// </summary>
        /// <param name="go">生成的对象</param>
        /// <param name="prefab_tag">对象标签</param>
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            //去除其他的所有 需求基础 的标签, 仅使用 tag
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

            //添加声音成本
            go.AddOrGet<LoopingSounds>();

            //添加工业机械标签
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);

            //添加标签 -> 发射按钮火箭模块
            go.GetComponent<KPrefabID>().AddTag(GameTags.LaunchButtonRocketModule, false);

            // 工作分配组控制器 启动时生成组 = 真
            go.AddOrGet<AssignmentGroupController>().generateGroupOnStart = true;

            // 乘客火箭模块 获取一个中等火箭内部空间标签
            go.AddOrGet<PassengerRocketModule>().interiorReverbSnapshot = AudioMixerSnapshots.Get().MediumRocketInteriorReverbSnapshot;

            // 添加火箭外部的门 -> 内部模板的名字使用和id同名
            // 火箭内部的世界在此脚本中生成
            go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = habitatData.TemplateName;

            //添加一个简单的门控制器
            go.AddOrGetDef<SimpleDoorController.Def>();

            // 添加传送器脚本?
            go.AddOrGet<NavTeleporter>();

            //路径控制器
            go.AddOrGet<AccessControl>();

            //可发射火箭群
            go.AddOrGet<LaunchableRocketCluster>();

            //火箭命令
            go.AddOrGet<RocketCommandConditions>();

            //火箭工艺条件显示目标
            go.AddOrGet<RocketProcessConditionDisplayTarget>();

            //字符覆盖
            go.AddOrGet<CharacterOverlay>().shouldShowName = true;

            //建筑附着点 0,4
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 4), GameTags.Rocket, null)
            };

            //添加储存脚本
            Storage storage = go.AddComponent<Storage>();

            //不显示 UI
            storage.showInUI = false;

            //容量 -> 10kg
            storage.capacityKg = 10f;

            //添加 火箭管道发送器
            RocketConduitSender rocketConduitSender = go.AddComponent<RocketConduitSender>();

            //引用火箭管道容器
            rocketConduitSender.conduitStorage = storage;

            //管道端口信息 -> 
            rocketConduitSender.conduitPortInfo = this.liquidInputPort;

            //火箭导管接收器
            go.AddComponent<RocketConduitReceiver>().conduitPortInfo = this.liquidOutputPort;

            //再添加一个储存空间
            Storage storage2 = go.AddComponent<Storage>();

            //不显示 UI
            storage2.showInUI = false;

            //容量 -> 1kg
            storage2.capacityKg = 1f;

            //添加 火箭管道发送器2
            RocketConduitSender rocketConduitSender2 = go.AddComponent<RocketConduitSender>();
            rocketConduitSender2.conduitStorage = storage2;
            rocketConduitSender2.conduitPortInfo = this.gasInputPort;
            go.AddComponent<RocketConduitReceiver>().conduitPortInfo = this.gasOutputPort;

        }

        /// <summary>
        /// 添加4个端口
        /// </summary>
        /// <param name="go"></param>
        private void AttachPorts(GameObject go)
        {
            go.AddComponent<ConduitSecondaryInput>().portInfo = this.liquidInputPort;
            go.AddComponent<ConduitSecondaryOutput>().portInfo = this.liquidOutputPort;
            go.AddComponent<ConduitSecondaryInput>().portInfo = this.gasInputPort;
            go.AddComponent<ConduitSecondaryOutput>().portInfo = this.gasOutputPort;
        }

        /// <summary>
        /// 在配置完成时执行
        /// </summary>
        /// <param name="go">传入要配置的对象</param>
        public override void DoPostConfigureComplete(GameObject go)
        {
            //将建筑拓展到火箭模板
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MAJOR, 0f, 0f);

            //添加可被持有的脚本
            Ownable ownable = go.AddOrGet<Ownable>();

            //插入火箭仓插槽
            ownable.slotID = Db.Get().AssignableSlots.HabitatModule.Id;

            //不允许此项被公有
            ownable.canBePublic = false;

            //添加假地板
            FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
            fakeFloorAdder.floorOffsets = new CellOffset[]
            {
            new CellOffset(-2, -1),
            new CellOffset(-1, -1),
            new CellOffset(0, -1),
            new CellOffset(1, -1),
            new CellOffset(2, -1)
            };

            //
            fakeFloorAdder.initiallyActive = false;

            //添加建筑格子查看器
            go.AddOrGet<BuildingCellVisualizer>();

            //添加可被重新排序的建筑 脚本
            go.GetComponent<ReorderableBuilding>().buildConditions.Add(new LimitOneCommandModule());
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            go.AddOrGet<BuildingCellVisualizer>();
            this.AttachPorts(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<BuildingCellVisualizer>();
            this.AttachPorts(go);
        }

        private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-2, 0));

        private ConduitPortInfo gasOutputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(2, 0));

        private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(-2, 3));

        private ConduitPortInfo liquidOutputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(2, 3));
    }
}
