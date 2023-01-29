using AsTool.Unity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Component.UIComponent.CommonComponent.Groupable
{
    /// <summary>
    /// 将组件分组，使得该组中只有且总有一个组件处于特殊状态，其余组件处于默认状态
    /// </summary>
    public class AsGroupable : AsMonoBehaviour
    {
        /// <summary>
        /// 所有在组内的组件
        /// </summary>
        private static readonly Dictionary<string, HashSet<AsGroupable>> groups = new Dictionary<string, HashSet<AsGroupable>>();

        /// <summary>
        /// 当前处于激活状态的组件
        /// </summary>
        private static readonly Dictionary<string, AsGroupable> specialOne = new Dictionary<string, AsGroupable>();

        /// <summary>
        /// 获取某组的特殊项
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>返回特殊项, 如果没有该组, 则返回null</returns>
        public static AsGroupable GetSpecialOne(string groupName)
        {
            if(specialOne.TryGetValue(groupName, out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 获取某组的下一个普通项并使其特殊化
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>返回被特殊化的项, 如果没有该组, 则返回null</returns>
        public static AsGroupable SpeciallizeNext(string groupName)
        {
            if (!specialOne.TryGetValue(groupName, out var value) || !groups.TryGetValue(groupName, out var hashset))
            {
                return null;
            }

            var list = hashset.ToList();

            int index = list.IndexOf(value) + 1;

            if(index < list.Count)
            {
                list[index].ToSpecial();

                return list[index];
            }
            else
            {
                list[0].ToSpecial();

                return list[0];
            }

        }

        /// <summary>
        /// 通过游戏对象的名字指定特殊化某个组内成员
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="gameObjectName">名字</param>
        /// <returns>返回被特殊化的项, 如果没有该组, 或找不到对应组员, 则返回null</returns>
        public static AsGroupable Speciallize(string groupName, string gameObjectName)
        {
            if (!specialOne.TryGetValue(groupName, out var value) || !groups.TryGetValue(groupName, out var hashset))
            {
                return null;
            }

            if (value.gameObject.name == gameObjectName)
                return value;

            foreach(var item in hashset)
            {
                if(item.gameObject.name == gameObjectName)
                {
                    item.ToSpecial();
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 当前所在组的名称, 仅在初始化前设置有效，否则使用<see cref="SetGroupName"/>
        /// </summary>
        [Tooltip("当前所在组的名称")]
        public string GroupName;

        /// <summary>
        /// 初始化时激活的优先级，优先级越高，在初始化时越先被激活
        /// </summary>
        [Tooltip("初始化时激活的优先级，优先级越高，在初始化时越先被激活")]
        public int Priority;

        /// <summary>
        /// 使该脚本的组件进入特殊状态
        /// </summary>
        public virtual void ToSpecial()
        {
            if (SpecializeInGroup())
            {
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 使得组件目标进入特殊化时的对于私有静态组的操作
        /// </summary>
        protected bool SpecializeInGroup()
        {
            var old = specialOne[GroupName];

            if (old == this) { return false; }

            old.ToNormal();

            specialOne[GroupName] = this;

            return true;
        }

        /// <summary>
        /// 使该脚本的组件恢复默认状态
        /// </summary>
        protected virtual void ToNormal()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 在唤醒时构建关系
        /// </summary>
        protected override void OnAwake()
        {
            SetGroupName(GroupName);

            base.OnAwake();
        }

        /// <summary>
        /// 重新设置组名称，在更改组和初始化组时使用
        /// </summary>
        /// <param name="newGroupName">组的名称</param>
        public virtual void SetGroupName(string newGroupName)
        {
            _ = UnSubscribe();

            //如果将要设置的脚本不在任何组中，直接返回
            if (string.IsNullOrWhiteSpace(newGroupName))
                return;

            GroupName = newGroupName;

            //查看新组是否已经被注册
            if (groups.TryGetValue(GroupName, out var newGroupables))
            {
                if (specialOne[GroupName].Priority <= Priority)
                {
                    specialOne[GroupName].ToNormal();

                    ToSpecial();

                    specialOne[GroupName] = this;
                }
                else
                {
                    ToNormal();
                }

                newGroupables.Add(this);
            }
            else
            {
                groups.Add(GroupName, new HashSet<AsGroupable>() { this });

                specialOne.Add(GroupName, this);

                ToSpecial();
            }
        }

        /// <summary>
        /// 取消本组件在组内的注册
        /// </summary>
        /// <returns>是否找到并取消注册</returns>
        private bool UnSubscribe()
        {
            //如果脚本本来就不在任何组中， 直接返回
            if (string.IsNullOrWhiteSpace(GroupName))
                return true;

            //查看是否已经注册了
            if (groups.TryGetValue(GroupName, out var groupables) && groupables.Contains(this))
            {
                groupables.Remove(this);

                ToNormal();

                //如果已经注册，查看现在是否活跃
                if (specialOne.TryGetValue(GroupName, out var active) && active == this)
                {
                    if (groupables.Any())
                    {
                        var newActive = groupables.First();

                        newActive.ToSpecial();

                        specialOne[GroupName] = newActive;
                    }
                    else
                    {
                        specialOne.Remove(GroupName);
                    }
                }

                if (!groupables.Any())
                {
                    groups.Remove(GroupName);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 在组件被销毁时，取消注册
        /// </summary>
        protected override void OnDestroy()
        {
            //如果程序正在退出, 会导致游戏对象先于脚本被移除，导致错误， 故直接返回
            if (AsApplicationCondition.IsQuitting)
                return;

            _ = UnSubscribe();

            base.OnDestroy();
        }
    }
}
