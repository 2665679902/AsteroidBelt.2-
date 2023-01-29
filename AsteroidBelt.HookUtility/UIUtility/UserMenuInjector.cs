using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidBelt.HookUtility.UIUtility
{
    /// <summary>
    /// 用户面板注入工具
    /// </summary>
    public static class UserMenuInjector
    {
        /// <summary>
        /// 在观察界面中添加一个按钮
        /// </summary>
        /// <param name="gameObject">按钮点击时信息由该对象发出</param>
        /// <param name="iconName">标签的名字</param>
        /// <param name="text">按钮的文本</param>
        /// <param name="tooltipText">提示文本</param>
        /// <param name="on_click">点击时触发的事件</param>
        /// <param name="shortcutKey">触发的快捷键</param>
        /// <param name="on_create">在创建时触发的函数</param>
        /// <param name="texture">纹理</param>
        /// <param name="is_interactable">是否是可交互的</param>
        /// <param name="sort_order">排列顺序, 越小越前面</param>
        public static void AddButton(
            GameObject gameObject = null, 
            string iconName = "", 
            string text = "",
            string tooltipText = "",
            System.Action on_click = null, 
            Action shortcutKey = Action.NumActions,
            Action<KIconButtonMenu.ButtonInfo> on_create = null, 
            Texture texture = null, 
            bool is_interactable = true, 
            float sort_order = 1f)
        {
            //on refresh is abandoned
            Game.Instance.userMenu.AddButton(
                gameObject, 
                new KIconButtonMenu.ButtonInfo(iconName, text, on_click, shortcutKey, null, on_create, texture, tooltipText, is_interactable), 
                sort_order);
        }

        
    }
}
