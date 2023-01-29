using AsTool.Load;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.HookUtility
{
    /// <summary>
    /// 本模组的入口函数信息
    /// </summary>
    public class Entrance : KMod.UserMod2
    {
        /// <summary>
        /// 本模组使用的修补实例
        /// </summary>
        public static Harmony EntranceHarmony { get; set; }

        /// <summary>
        /// 模组导入时加载
        /// </summary>
        /// <param name="harmony"></param>
        public override void OnLoad(Harmony harmony)
        {
            EntranceHarmony = harmony;

            AsLoadManager.StartLoad();

            EntranceHarmony.PatchAll();
        }
    }
}
