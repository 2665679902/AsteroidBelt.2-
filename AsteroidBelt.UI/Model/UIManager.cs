using AsteroidBelt.Data.String;
using AsteroidBelt.UI.EventHandler.ButtonEvents;
using AsteroidBelt.UI.UIEvent;
using AsTool.Event;
using AsTool.Extension;
using AsTool.Load;
using AsTool.Unity.AssetBundleManager;
using AsTool.Unity.Common;
using AsTool.Unity.Component.EventComponent;
using AsTool.Unity.Component.UIComponent.CommonComponent.Groupable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidBelt.UI.Model
{
    /// <summary>
    /// UI管理工具类
    /// </summary>
    public static class UIManager
    {
        /// <summary>
        /// UI 的面板
        /// </summary>
        public static Canvas ModUICanvas { get; private set; }

        /// <summary>
        /// 加载时创建UI组件
        /// </summary>
        [AsLoad]
        internal static void Load()
        {
            var last = GameObject.Find(CodeStringConfig.UIString.CanvasName);

            if(last != null)
            {
                UnityEngine.Object.Destroy(last);
            }

            ModUICanvas = AsAssetBundles.GetInstantiatedGameObject(CodeStringConfig.UIString.CanvasName).GetComponent<Canvas>();

            AsPersistentGameObject.SetChild(ModUICanvas.gameObject);

            AsGroupable.GetSpecialOne(CodeStringConfig.UIString.PanelGroupName.RootLevel).gameObject.SetActive(false);
        }

        /// <summary>
        /// 开启根面板
        /// </summary>
        public static void OpenRootPanel()
        {
            AsGroupable.GetSpecialOne(CodeStringConfig.UIString.PanelGroupName.RootLevel).gameObject.SetActive(true);
        }

        /// <summary>
        /// 开启设计火箭面板的界面
        /// </summary>
        public static void OpenHabitatDesignPanel()
        {
            OpenRootPanel();
        }
    }
}
