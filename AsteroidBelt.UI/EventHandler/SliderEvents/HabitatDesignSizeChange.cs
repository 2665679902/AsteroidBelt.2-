using AsteroidBelt.Data.String.Messages;
using AsteroidBelt.UI.Component.ViewComponent;
using AsteroidBelt.UI.UIEvent;
using AsTool.Event;
using AsTool.Extension;
using AsTool.Unity.Component.EventComponent;
using AsTool.Unity.Component.UIComponent.DefaultUIComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidBelt.UI.EventHandler.SliderEvents
{
    /// <summary>
    /// 处理事件<see cref="SliderEvent.HabitatDesignSizeChange"/>
    /// </summary>
    internal class HabitatDesignSizeChange : AsSliderEventHandler
    {
        protected override SliderEvent Event => SliderEvent.HabitatDesignSizeChange;


        public override AsComponentEventArg Action(AsComponentEventArg data)
        {
            if (data.Sender is AsSlider slider)
            {
                HabitatDropdownGroup.Instance.SetSize((int)slider.Value);

                AsEvent.Trigger(InputFieldEvent.Get_HabitatSizeText.AsToString(), new AsComponentEventArg() { Sender = data.Sender, Data = (int)slider.Value });
            }

            return data;
        }
    }
}
