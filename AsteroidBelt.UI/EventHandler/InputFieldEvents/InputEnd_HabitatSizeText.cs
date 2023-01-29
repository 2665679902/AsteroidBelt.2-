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

namespace AsteroidBelt.UI.EventHandler.InputFieldEvents
{
    /// <summary>
    /// 处理事件<see cref="InputFieldEvent.InputEnd_HabitatSizeText"/>
    /// </summary>
    internal class InputEnd_HabitatSizeText : AsInputFieldEventHandler
    {
        protected override InputFieldEvent Event => InputFieldEvent.InputEnd_HabitatSizeText;

        public override AsComponentEventArg Action(AsComponentEventArg data)
        {
            if(data.Sender is AsInputField field)
            {
                if(int.TryParse(field.InputText, out int result))
                {
                    if(result > 60)
                    {
                        result = 60;
                    }
                    else if(result < 10)
                    {
                        result = 10;
                    }

                    field.InputText = result.ToString();

                    HabitatDropdownGroup.Instance.SetSize(result);

                    AsEvent.Trigger(SliderEvent.Get_HabitatDesignSizeChange.AsToString(), new AsComponentEventArg() {Sender = data.Sender, Data = (float)result });
                }
            }

            return data;
        }
    }
}
