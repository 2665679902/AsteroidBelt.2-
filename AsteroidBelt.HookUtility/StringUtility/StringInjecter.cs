using AsteroidBelt.Data.String;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.HookUtility.StringUtility
{
    /// <summary>
    /// ONI文字注入工具
    /// </summary>
    public static class StringInjecter
    {
        /// <summary>
        /// 监视加载翻译文件
        /// </summary>
        [HarmonyPatch(typeof(Localization), "LoadLocalTranslationFile")]
        private static class Localization_Patch
        {
            /// <summary>
            /// 在翻译文件加载后执行
            /// </summary>
            public static void Postfix()
            {
                AsLog.Info("ONI加载翻译文件完成 Localization LoadLocalTranslation");

                AsString.LoadFromPo(CurrentLanguage);

                LocalizationFinished?.Invoke();
            }
        }

        /// <summary>
        /// 当前的语言类型
        /// </summary>
        public static AsString.Language CurrentLanguage 
        { 
            get
            {
                var lang = Localization.GetLocale()?.Lang ?? Localization.Language.Unspecified;

                switch (lang)
                {
                    case Localization.Language.Chinese: return AsString.Language.Chinese;
                    case Localization.Language.Japanese: return AsString.Language.Japanese;
                    case Localization.Language.Korean: return AsString.Language.Korean;
                    case Localization.Language.Russian: return AsString.Language.Russian;
                    case Localization.Language.Thai: return AsString.Language.Thai;
                    case Localization.Language.Arabic: return AsString.Language.Arabic;
                    case Localization.Language.Hebrew: return AsString.Language.Hebrew;
                    case Localization.Language.Unspecified: return AsString.Language.Unspecified;
                    default:
                        return AsString.Language.Unspecified;
                }
            } 
        }

        /// <summary>
        /// 翻译文件加载完成事件(此时语言类型最终确定)
        /// </summary>
        public static event System.Action LocalizationFinished;
    }
}
