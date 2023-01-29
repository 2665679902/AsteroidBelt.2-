using AsteroidBelt.Data.EventTokens;
using AsteroidBelt.Data.String;
using AsteroidBelt.Data.String.Messages;
using AsteroidBelt.UI.Component.ViewComponent;
using AsteroidBelt.UI.UIEvent;
using AsTool.Event;
using AsTool.Extension;
using AsTool.Unity.Component.EventComponent;
using AsTool.Unity.Component.UIComponent.CommonComponent.Groupable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.UI.EventHandler.ButtonEvents
{
    /// <summary>
    /// 处理事件<see cref="ButtonEvent.Close_ModConfig_Root"/>
    /// </summary>
    internal class Close_ModConfig_Root : AsButtonEventHandler
    {
        protected override ButtonEvent Event => ButtonEvent.Close_ModConfig_Root;

        public override AsComponentEventArg Action(AsComponentEventArg data)
        {
            AsGroupable.GetSpecialOne(CodeStringConfig.UIString.PanelGroupName.RootLevel).gameObject.SetActive(false);

            return data;
        }
    }
}
