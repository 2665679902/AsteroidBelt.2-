using AsteroidBelt.Data.String.Messages;
using AsteroidBelt.HookUtility.BuildingUtility;
using AsteroidBelt.HookUtility.UIUtility;
using AsteroidBelt.UI.Model;
using AsTool.Load;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.Component.Building.LaunchPad
{
    /// <summary>
    /// 用于添加设计太空舱的按钮
    /// </summary>
    public class AddDesignHabitatButton: AsKMonobehavior
    {
        [AsLoad]
        internal static void Load()
        {
            LaunchPadConfigPatch.AfterConfigureBuildingTemplate += LaunchPadConfigPatch_AfterConfigureBuildingTemplate;
        }

        private static void LaunchPadConfigPatch_AfterConfigureBuildingTemplate(UnityEngine.GameObject arg1, Tag arg2)
        {
            arg1.AddOrGet<AddDesignHabitatButton>();
        }







        /// <summary>
        /// 在生成时添加按钮
        /// </summary>
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            _ = Subscribe(GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
        }

        /// <summary>
		/// 刷新主要UI的时候执行此事件
		/// </summary>
		private static readonly EventSystem.IntraObjectHandler<AddDesignHabitatButton> OnRefreshUserMenuDelegate
            = new EventSystem.IntraObjectHandler<AddDesignHabitatButton>(
                (AddDesignHabitatButton component, object data) => component.OnRefreshUserMenu(data));

        /// <summary>
        /// 刷新UserMenu的时候添加设计按钮
        /// </summary>
        /// <param name="data">触发的游戏对象, 不使用</param>
        private void OnRefreshUserMenu(object data)
        {
            UserMenuInjector.AddButton(
                gameObject: gameObject, 
                iconName: "action_navigable_regions",
                text: AsHabitateString.UI.Button.DesignButton,
                tooltipText: AsHabitateString.UI.Button.DesignButtonTooltipText,
                on_click: UIManager.OpenHabitatDesignPanel
                );
        }
    }
}
