using AsteroidBelt.UI.Component.ViewComponent;
using AsteroidBelt.UI.UIEvent;
using AsTool.Unity.Component.EventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.UI.EventHandler.ButtonEvents
{
    /// <summary>
    /// 处理事件<see cref="ButtonEvent.HabitatDropdownGroupSetDefault"/>
    /// </summary>
    internal class HabitatDropdownGroupSetDefault : AsButtonEventHandler
    {
        protected override ButtonEvent Event => ButtonEvent.HabitatDropdownGroupSetDefault;

        public override AsComponentEventArg Action(AsComponentEventArg data)
        {
            HabitatDropdownGroup.Instance.SetDefault();

            return data;
        }
    }
}
