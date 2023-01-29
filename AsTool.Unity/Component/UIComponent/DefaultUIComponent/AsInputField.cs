using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsTool.Unity.Component.UIComponent.DefaultUIComponent
{
    /// <summary>
    /// 一个管理<see cref="InputField"/>的组件
    /// </summary>
    public class AsInputField : AsUIComponent
    {
        /// <summary>
        /// 内部管理的组件
        /// </summary>
        public InputField InputField { get => GetComponent<InputField>(); }

        /// <summary>
        /// 当前文本框中的内容
        /// </summary>
        public string InputText { get => InputField.text; set => InputField.text = value; }

        /// <summary>
        /// 输入的限制字符, 如果没有限制, 则为0
        /// </summary>
        public int CharacterLimit { get => InputField.characterLimit; set => InputField.characterLimit = value; }

        /// <summary>
        /// 允许输入的字符模式
        /// </summary>
        public InputField.ContentType ContentType { get => InputField.contentType; set => InputField.contentType = value; }

        /// <summary>
        /// 输入的行类型
        /// </summary>
        public InputField.LineType LineType { get => InputField.lineType; set => InputField.lineType = value; }

        /// <summary>
        /// 空时显示的文本 [注意: 当目标对象没有文本组件时 调用此属性会报错]
        /// </summary>
        public string PlaceholderText { get => InputField.placeholder.GetComponent<Text>().text; set => InputField.placeholder.GetComponent<Text>().text = value; }

        /// <summary>
        /// 该文本框是否是只读的
        /// </summary>
        public bool ReadOnly { get => InputField.readOnly; set => InputField.readOnly = value; }

        /// <summary>
        /// 在初始化时绑定输入完成事件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            InputField.onEndEdit.AddListener(OnEndEdit);
        }

        /// <summary>
        /// 当编辑结束(失去焦点)时触发此函数 【有bug 记得修】
        /// </summary>
        /// <param name="value">当前的字符</param>
        public virtual void OnEndEdit(string value)
        {
            try
            {
                EventTrigger?.Trigger(this);
            }
            catch(Exception e) { AsLog.Fatal(e); }

        }
    }
}
