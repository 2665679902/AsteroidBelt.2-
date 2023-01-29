using AsTool.Assert;
using AsTool.Unity.Component.UIComponent.DefaultUIComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Component.UIComponent.CommonComponent.Groupable
{
    ///// <summary>
    ///// 在一组下拉框中, 某一个(或几个)特殊的值仅允许某一个(或几个特殊的下拉框选中)
    ///// </summary>
    //public class AsGroupableDropdown : AsDropdown, IComparable<AsGroupableDropdown>
    //{
    //    /// <summary>
    //    /// 所有在组内的组件
    //    /// </summary>
    //    private static readonly Dictionary<string, HashSet<AsGroupableDropdown>> groups = new Dictionary<string, HashSet<AsGroupableDropdown>>();

    //    /// <summary>
    //    /// 当前处于激活状态的组件
    //    /// </summary>
    //    private static readonly Dictionary<string, List<AsGroupableDropdown>> special = new Dictionary<string, List<AsGroupableDropdown>>();

    //    /// <summary>
    //    /// 当前所在组的名称, 仅在初始化前设置有效，否则使用<see cref="SetGroupName"/>
    //    /// </summary>
    //    [Tooltip("当前所在组的名称")]
    //    public string GroupName;

    //    /// <summary>
    //    /// 初始化时激活的优先级，优先级越高，在初始化时越先被激活
    //    /// </summary>
    //    [Tooltip("初始化时激活的优先级，优先级越高，在初始化时越先被激活")]
    //    public int Priority = 0;

    //    /// <summary>
    //    /// 各个索引许可的最大数量(如果小于0 , 则表示数量不限)
    //    /// </summary>
    //    [Tooltip("各个索引许可的最大数量(如果小于0 , 则表示数量不限, 等于0则表示不可设置)")]
    //    public List<int> SpecialAmount = new List<int>();

    //    /// <summary>
    //    /// 当下拉框被从特殊位置取消时会设置为此索引, 此索引的数量不可设限(不然会报错)
    //    /// </summary>
    //    [Tooltip("当下拉框被从特殊位置取消时会设置为此索引, 此索引的数量不可设限(不然会报错)")]
    //    public int DefaultIndex = 0;

    //    /// <summary>
    //    /// 该脚本的组件进入特殊状态时触发
    //    /// </summary>
    //    public virtual void ToSpecial()
    //    {
    //    }

    //    /// <summary>
    //    /// 使该下拉框恢复默认状态
    //    /// </summary>
    //    private void ToNormal()
    //    {
    //        Value = DefaultIndex;
    //    }

    //    /// <summary>
    //    /// 重新设置组名称，在更改组和初始化组时使用
    //    /// </summary>
    //    /// <param name="newGroupName">组的名称</param>
    //    public virtual void SetGroupName(string newGroupName)
    //    {
    //        _ = UnSubscribe();

    //        GroupName = newGroupName;

    //        //添加入相应的组中
    //        if (!groups.TryGetValue(GroupName, out var hashSet))
    //        {
    //            groups.Add(GroupName, hashSet = new HashSet<AsGroupableDropdown>());
    //        }
    //        hashSet.Add(this);

    //        //获取当前的特殊组
    //        if (!special.TryGetValue(GroupName, out var specialList))
    //        {
    //            special.Add(GroupName, specialList = new List<AsGroupableDropdown>());
    //        }

    //        //如果当前处在特殊位置
    //        if (Value != DefaultIndex)
    //        {
    //            //如果设置了当前数量限制, 并且如果当前许可的特殊组件数量为0
    //            if (SpecialAmount.Count > Value && SpecialAmount[Value] == 0)
    //            {
    //                //直接将本组件恢复至默认状态
    //                ToNormal();
    //                return;
    //            }


    //            //如果设置了当前数量限制, 并且当前数量限制 > 0
    //            if (SpecialAmount.Count > Value && SpecialAmount[Value] > 0)
    //            {
    //                //获取当前特殊化的同索引组件
    //                var sList = specialList.Where((dp) => dp.Value == Value).ToList();
    //                sList.Sort();

    //                //如果当前目标特殊组已经达到限制
    //                for (int count = sList.Count; count >= SpecialAmount[Value]; count--)
    //                {
    //                    //普通化第一个找到的特殊组件
    //                    var item = sList.First((dp) => dp.Value == Value);
    //                    specialList.Remove(item);
    //                    item.ToNormal();
    //                }
    //            }

    //            specialList.Add(this);

    //            ToSpecial();
    //        }
    //    }

    //    /// <summary>
    //    /// 移除本组件在原组中的注册
    //    /// </summary>
    //    /// <returns>是否成功移除</returns>
    //    public virtual bool UnSubscribe()
    //    {
    //        if (!string.IsNullOrWhiteSpace(GroupName) && //如果原群组是空白的, 直接返回
    //            groups.TryGetValue(GroupName, out var hashSet) && //如果原群组不存在, 直接返回
    //            hashSet.Remove(this) && //如果移除失败, 直接返回
    //            special.TryGetValue(GroupName, out var specialList) && //如果特殊群组中找不到, 直接返回
    //            specialList.Remove(this))//如果移除成功返回真
    //        {
    //            Refresh();

    //            return true;
    //        }

    //        Refresh();

    //        return false;
    //    }

    //    /// <summary>
    //    /// 刷新本脚本的状态
    //    /// </summary>
    //    public void Refresh()
    //    {
    //        //获取当前的特殊组
    //        if (!special.TryGetValue(GroupName, out var specialList))
    //        {
    //            special.Add(GroupName, specialList = new List<AsGroupableDropdown>());

    //            return;
    //        }

    //        //将本脚本移动到合适的组内
    //        if (Value == DefaultIndex && specialList.Contains(this))
    //        {
    //            specialList.Remove(this);
    //        }
    //        else if (Value != DefaultIndex && !specialList.Contains(this))
    //        {
    //            specialList.Add(this);
    //        }

    //        //如果设置了当前数量限制, 并且当前数量限制 >= 0
    //        if (SpecialAmount.Count > Value && SpecialAmount[Value] >= 0)
    //        {
    //            //如果当前目标特殊组已经达到限制
    //            for (int count = specialList.Where((dp) => dp.Value == Value).Count(); count > SpecialAmount[Value]; count--)
    //            {
    //                //普通化第一个找到的特殊组件, 直到数量符合要求
    //                var item = specialList.First((dp) => dp.Value == Value);
    //                specialList.Remove(item);
    //                item.ToNormal();
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 在构建时调用
    //    /// </summary>
    //    protected override void OnAwake()
    //    {
    //        base.OnAwake();

    //        if (SpecialAmount.Count() > DefaultIndex && SpecialAmount[DefaultIndex] > 0)
    //            AsAssert.Fatal("AsGroupableDropdown 默认索引的数量不可设限");

    //        SetGroupName(GroupName);

    //        Refresh();
    //    }

    //    /// <summary>
    //    /// 当值改变时, 重新判断当前组的状态
    //    /// </summary>
    //    /// <param name="value">当前的下拉列表的索引值</param>
    //    public override void OnValueChanged(int value)
    //    {
    //        base.OnValueChanged(value);

    //        //获取当前的特殊组
    //        if (!special.TryGetValue(GroupName, out var specialList))
    //        {
    //            special.Add(GroupName, specialList = new List<AsGroupableDropdown>());
    //        }

    //        //如果目标现在需要在 且 正在特殊组内
    //        if (Value != DefaultIndex && specialList.Remove(this))
    //        {
    //            //将目标移动至特殊组的末尾
    //            specialList.Add(this);
    //        }

    //        Refresh();

    //        special[GroupName].Sort();
    //    }

    //    /// <summary>
    //    /// 在组件被销毁时，取消注册
    //    /// </summary>
    //    protected override void OnDestroy()
    //    {
    //        _ = UnSubscribe();

    //        base.OnDestroy();
    //    }

    //    /// <summary>
    //    /// 比较大小时, 通过其优先级比较
    //    /// </summary>
    //    /// <param name="other">其他的脚本</param>
    //    /// <returns>比较结果</returns>
    //    int IComparable<AsGroupableDropdown>.CompareTo(AsGroupableDropdown other)
    //    {
    //        return Priority.CompareTo(other.Priority);
    //    }
    //}
}
