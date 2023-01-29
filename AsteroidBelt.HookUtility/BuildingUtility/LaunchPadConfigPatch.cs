using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidBelt.HookUtility.BuildingUtility
{
    /// <summary>
    /// 介入火箭发射底座的生成
    /// </summary>
    public static class LaunchPadConfigPatch
    {
        /// <summary>
        /// 在建筑配置文件创建后触发
        /// </summary>
        public static event Action<BuildingDef> AfterCreateBuildingDef;

        /// <summary>
        /// 在建筑实例生成后触发
        /// </summary>
        public static event Action<GameObject, Tag> AfterConfigureBuildingTemplate;

        [HarmonyPatch(typeof(LaunchPadConfig), nameof(LaunchPadConfig.CreateBuildingDef))]
        private static class CreateBuildingDef
        {
            public static void Postfix(ref BuildingDef __result)
            {
                AfterCreateBuildingDef?.Invoke(__result);
            }
        }

        [HarmonyPatch(typeof(LaunchPadConfig), nameof(LaunchPadConfig.ConfigureBuildingTemplate))]
        private static class ConfigureBuildingTemplate
        {
            public static void Postfix(GameObject go, Tag prefab_tag)
            {
                AfterConfigureBuildingTemplate?.Invoke(go, prefab_tag);
            }
        }
    }
}
