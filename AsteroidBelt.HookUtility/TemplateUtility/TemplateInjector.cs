using AsteroidBelt.Data;
using AsteroidBelt.Data.String;
using HarmonyLib;
using AsteroidBelt.Data.templates;
using System.Collections.Generic;
using AsteroidBelt.DataBuilder;
using TUNING;
using AsteroidBelt.Data.EventTokens;
using AsTool.Event;
using AsTool.Load;
using AsteroidBelt.Data.templates.Building.Rocket.Habitat;

namespace AsteroidBelt.HookUtility.TemplateUtility
{
    /// <summary>
    /// ONI template 注入工具
    /// </summary>
    public static class TemplateInjector
    {
        /// <summary>
        /// 存放所有模板的容器
        /// </summary>
        private static Dictionary<string, TemplateContainer> templates;

        /// <summary>
        /// 存放所有模板的容器, <see cref="TemplateCache"/>没有初始化时返回 null
        /// </summary>
        public static Dictionary<string, TemplateContainer> Templates
        {
            get
            {
                if (!TemplateCache.Initted)
                    return null;

                if (templates is null)
                    templates = Traverse.Create(typeof(TemplateCache)).Field("templates").GetValue() as Dictionary<string, TemplateContainer>;

                return templates;
            }
        }

        /// <summary>
        /// 从文件刷新所有的<see cref="AsTemplatesObject"/>(手动写的)
        /// </summary>
        public static void RefreshTemplateData()
        {
            if (!TemplateCache.Initted)
                return;

            //刷新太空舱数据
            var data = AsDataObject.RefreshFromFile<HugeHabitatData>();

            if (data is null)
            {
                AsLog.Error("HugeHabitatData lost. Is this the first time to start?");
            }
            else
            {
                var HugeHabitatData = AsTemplatesObjectBuilder.GetContainer(data);
                Templates[HugeHabitatData.name] = HugeHabitatData;
                Templates["expansion1::interiors/habitat_huge"] = HugeHabitatData;

                //适配太空舱空间尺寸
                var bounds = HugeHabitatData.info.GetBounds(new Vector2I(), 2);
                ROCKETRY.ROCKET_INTERIOR_SIZE = new Vector2I(bounds.width > 32 ? bounds.width : 32, bounds.height > 32 ? bounds.height : 32);
            }



            AsLog.Info("模板数据注入完成 template data injection completed");
        }

        [AsLoad]
        internal static void Load()
        {
            AsEvent.Subscribe(DataEvents.RefreshTemplateData, RefreshTemplateData);

            if (TemplateCache.Initted)
            {
                RefreshTemplateData();
            }
        }

        /// <summary>
        /// 介入Tamplate的行为
        /// </summary>
        private static class TemplateCache_Patch
        {
            [HarmonyPatch(typeof(TemplateCache), "RewriteTemplatePath")]
            private static class RewriteTemplatePath
            {
                public static bool Prefix(string scopePath, ref string __result)
                {
                    if (scopePath.StartsWith(CodeStringConfig.TitleString.AsTemplatesObjectTitle))
                    {
                        AsLog.Error("WARMING! some one try to get the fake path, I have a premonition that something bad will happen");

                        __result = AsDataObject.GetFullPath(scopePath);

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            [HarmonyPatch(typeof(TemplateCache), "Init")]
            private static class Init
            {
                public static void Postfix()
                {
                    AsLog.Info("模板缓存管理初始化 TemplateCache Init.");

                    RefreshTemplateData();
                }
            }

            //[HarmonyPatch(typeof(TemplateCache), "GetTemplate")]
            //private static class GetTemplate
            //{
            //    public static void Prefix(string templatePath)
            //    {
            //        AsLog.Debug("GetTemplate " + templatePath);
            //    }
            //}

            [HarmonyPatch(typeof(TemplateCache), "TemplateExists")]
            private static class TemplateExists
            {
                public static void Prefix(string templatePath, ref bool __result)
                {
                    if (templatePath.StartsWith(CodeStringConfig.TitleString.AsTemplatesObjectTitle))
                    {
                        __result = true;
                    }
                }
            }
        }
    }
}
