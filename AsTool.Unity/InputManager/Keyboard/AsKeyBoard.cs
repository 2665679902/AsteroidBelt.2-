using AsTool.Assert;
using AsTool.Common.Extension;
using AsTool.Event;
using AsTool.Load;
using AsTool.Unity.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.InputManager.Keyboard
{
    /// <summary>
    /// 用于获取键盘事件的类(会无差别地捕获键盘信息)
    /// </summary>
    public static class AsKeyBoard
    {
        /// <summary>
        /// 对所有的<see cref="KeyCode"/>的项的引用
        /// </summary>
        private static readonly KeyCode[] allCodes = Enum.GetValues(typeof(KeyCode)).AsSelect(item => (KeyCode)item).ToArray();

        /// <summary>
        /// 内部生命周期函数对应方法的名字
        /// </summary>
        private const string LifeCycleActionName = "AsKeyBoardEvent_LifeCycleAction_Name";

        /// <summary>
        /// 保存100步的输入历史
        /// </summary>
        private static KeyCode[] CodesHistory = new KeyCode[100];

        /// <summary>
        /// 当前的索引
        /// </summary>
        private static int currentIndex = 0;

        /// <summary>
        /// 当有按键按下时触发此事件
        /// </summary>
        public static event Action GetKeyDown;

        /// <summary>
        /// 判断输入历史是否符合传入顺序
        /// </summary>
        /// <param name="keyCodes">传入的要判断是否符合的输入历史, 此枚举项不能大于 100 </param>
        /// <returns>如果输入顺序和传入顺序相同则返回真, 否则返回假</returns>
        public static bool HasHistory(IEnumerable<KeyCode> keyCodes)
        {
            AsAssert.NotNull(keyCodes, "keyCodes can not be null");
            AsAssert.IsTrue(keyCodes.Count() <= 100, "keyCodes can not longer than 100");

            int current = currentIndex - keyCodes.Count() + 1;

            if(current < 0) 
            {
                current += CodesHistory.Length;
            }

            foreach (KeyCode code in keyCodes)
            {
                if(code != CodesHistory[current])
                    return false;

                current++;

                if(current >= CodesHistory.Length)
                    current = 0;
            }

            return true;
        }

        /// <summary>
        /// 判断最后一个输入是否是目标值
        /// </summary>
        /// <param name="keyCode">要判断的值</param>
        /// <returns>是目标值返回真, 否则返回假</returns>
        public static bool HasHistory(KeyCode keyCode)
        {
            return CodesHistory[currentIndex] == keyCode;
        }

        /// <summary>
        /// 在刷新时执行此函数
        /// </summary>
        private static void OnUpdate()
        {
            if (Input.anyKeyDown)
            {
                foreach(var code in allCodes)
                {
                    if(Input.GetKeyDown(code))
                    {
                        currentIndex++;

                        if (currentIndex >= CodesHistory.Length)
                            currentIndex = 0;

                        CodesHistory[currentIndex] = code;

                        break;
                    }
                }

                GetKeyDown?.Invoke();
            }
        }

        /// <summary>
        /// 在dll启动时加载
        /// </summary>
        [AsLoad]
        internal static void Load()
        {
            AsAssert.IsTrue(AsCommonMono.SetLifeCycleAction(LifeCycleActionName, OnUpdate, AsCommonMono.LifeCycle.Update), "AsKeyBoard set Action failed");
        }
    }
}
