using AsteroidBelt.Data.EventTokens;
using AsteroidBelt.Data.String.Messages;
using AsteroidBelt.UI.Component.ViewComponent;
using AsteroidBelt.UI.UIEvent;
using AsTool.Event;
using AsTool.Extension;
using AsTool.Unity.Component.EventComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.UI.EventHandler.ButtonEvents
{
    /// <summary>
    /// 处理事件<see cref="ButtonEvent.HabitatDropdownGroupSave"/>
    /// </summary>
    internal class HabitatDropdownGroupSave : AsButtonEventHandler
    {
        protected override ButtonEvent Event => ButtonEvent.HabitatDropdownGroupSave;

        public override AsComponentEventArg Action(AsComponentEventArg data)
        {
            HabitatDropdownGroup.Instance.Save(out var message);

            AsEvent.Trigger(TextEvent.Get_HabitatConditionText.AsToString(), new AsComponentEventArg() { Sender = data.Sender, Data = string.IsNullOrEmpty(message) ? AsHabitateString.Success.HabitatSaveSuccess.Translate() : message });

            AsEvent.Trigger(DataEvents.RefreshTemplateData);

            return data;
        }
    }
}
