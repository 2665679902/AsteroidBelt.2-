using AsTool.Assert;
using AsTool.Log;
using AsTool.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Common.Extension
{
    /// <summary>
    /// 可迭代项的一些拓展方法
    /// </summary>
    public static class AsEnumerableExtension
    {
        private static readonly Random random  = new Random();

        /// <summary>
        /// 获取某一固定位置的元素
        /// </summary>
        /// <param name="list">要查找的可迭代项</param>
        /// <param name="num">序列位置</param>
        /// <returns>找到返回，找不到返回null</returns>
        public static object AsElementAt(this IEnumerable list, int num)
        {
            AsAssert.NotNull(list, "AsElementAt: list or act");

            int length = 0;

            return list.AsFirst((o) => length++ == num);
        }

        /// <summary>
        /// 处理列表的每一项并输出 [ 优先使用 Foreach ]
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="list">输入的可迭代项</param>
        /// <param name="act">处理函数</param>
        /// <returns>原列表</returns>
        public static IEnumerable<T> AsForech<T>(this IEnumerable<T> list, Action<T> act)
        {

            AsAssert.NotNull(list, "AsForech: list");
            AsAssert.NotNull(act, "AsForech: act");

            foreach (var item in list)
            {
                act(item);
            }

            return list;
        }

        /// <summary>
        /// 处理列表的每一项并输出 [ 优先使用 Foreach ]
        /// </summary>
        /// <param name="list">输入的可迭代项</param>
        /// <param name="act">处理函数</param>
        /// <returns>原列表</returns>
        public static IEnumerable<T> AsForech<T>(this IEnumerable list, Action<T> act)
        {

            AsAssert.NotNull(list, "AsForech: list");
            AsAssert.NotNull(act, "AsForech: act");

            var result = new List<T>();

            foreach (var item in list)
            {
                act((T)item);
                result.Add((T)item);
            }

            return result;
        }

        /// <summary>
        /// 处理列表的每一项并输出新列表 [ 优先使用 Select]
        /// </summary>
        /// <param name="list">输入列表</param>
        /// <param name="func">处理函数</param>
        /// <returns>处理结果</returns>
        public static IEnumerable<object> AsSelect(this IEnumerable list, Func<object, object> func)
        {
            AsAssert.NotNull(list, "AsSelect: list");
            AsAssert.NotNull(func, "AsSelect: act");

            var result = new List<object>();

            foreach (var item in list)
            {
                result.Add(func(item));
            }

            return result;
        }

        /// <summary>
        /// 处理列表的每一项并输出新列表 [ 优先使用 Select]
        /// </summary>
        /// <param name="list">输入列表</param>
        /// <param name="func">处理函数</param>
        /// <returns>处理结果</returns>
        public static IEnumerable<T> AsSelect<T>(this IEnumerable list, Func<object, T> func)
        {
            AsAssert.NotNull(list, "AsSelect: list");
            AsAssert.NotNull(func, "AsSelect: act");

            var result = new List<T>();

            foreach (var item in list)
            {
                result.Add(func(item));
            }

            return result;
        }

        /// <summary>
        /// 在指定的集合中找到目标，在其后增加一项
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="list">原集合</param>
        /// <param name="target">目标</param>
        /// <param name="someThingAdd">增加项</param>
        /// <param name="strict">如果没找到是不是放弃添加 （否则会加载末尾）</param>
        /// <returns>如果成功找到目标并添加返回真， 否则返回假</returns>
        public static bool AsAddAfter<T>(this IList<T> list, T target, T someThingAdd, bool strict = false)
        {

            AsAssert.NotNull(list, "AsAddAfter: list");
            AsAssert.NotNull<object>(target, "AsAddAfter: target");
            AsAssert.NotNull<object>(someThingAdd, "AsAddAfter: someThingAdd");


            int index = list.IndexOf(target) + 1;

            if (index < 1)
            {
                if (!strict)
                    list.Append(someThingAdd);
                return false;
            }

            if (index < list.Count())
                list.Insert(index, someThingAdd);
            else
                list.Add(someThingAdd);

            return true;
        }

        /// <summary>
        /// 尝试移除列表的某一项
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="list">原集合</param>
        /// <param name="target">目标项</param>
        /// <returns>成功返回 true 否则返回 false</returns>
        /// <exception cref="ArgumentNullException">输入的参数都不能为 null</exception>
        public static bool AsTryRemove<T>(this IList<T> list, T target)
        {
            AsAssert.NotNull(list, "AsTryRemove: list");
            AsAssert.NotNull<object>(target, "AsTryRemove: target");

            if (!list.Contains(target))
                return false;

            list.Remove(target);
            return true;

        }

        /// <summary>
        /// 去除一层列表项的嵌套
        /// </summary>
        /// <typeparam name="T">要累加的</typeparam>
        /// <param name="list"></param>
        /// <returns>结果列表</returns>
        public static IEnumerable<T> AsRemoveNesting<T>(this IEnumerable<IEnumerable<T>> list)
        {
            List<T> list_result = new List<T>();

            foreach(var itemList in list)
            {
                foreach(var item in itemList)
                {
                    list_result.Add(item);
                }
            }

            return list_result;
        }

        /// <summary>
        /// 尝试在列表里寻找重复项
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="list">原集合</param>
        /// <returns>输出重复项的键值对列表</returns>
        public static List<KeyValuePair<T, T>> AsFindRepetition<T>(this IEnumerable<T> list)
        {
            AsAssert.NotNull(list, "AsFindRepetition: list");

            var result = new List<KeyValuePair<T, T>>();

            for (int i = 0; i < list.Count(); i++)
            {
                for (int j = i + 1; j < list.Count(); j++)
                {
                    if (list.ElementAt(i).Equals(list.ElementAt(j)))
                        result.Add(new KeyValuePair<T, T>(list.ElementAt(i), list.ElementAt(j)));
                }
            }

            return result;
        }

        /// <summary>
        /// 查找第一个符合项
        /// </summary>
        /// <param name="list">原集合</param>
        /// <param name="func">比较方法</param>
        /// <returns>第一个符合项，如果没有就返回 null</returns>
        /// <exception cref="ArgumentNullException">输入的参数都不能为 null</exception>
        public static object AsFirst(this IEnumerable list, Func<object, bool> func)
        {
            AsAssert.NotNull(list, "AsFirst: list");
            AsAssert.NotNull(func, "AsFirst: func");

            foreach (var item in list)
            {
                if (func(item))
                    return item;
            }

            return null;
        }

        /// <summary>
        /// 查找最后一个符合项
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="list">原集合</param>
        /// <param name="func">比较方法</param>
        /// <returns>最后符合项的序列号，如果没有就返回 -1</returns>
        /// <exception cref="ArgumentNullException">输入的参数都不能为 null</exception>
        public static int AsFindLast<T>(this IEnumerable<T> list, Func<T, bool> func)
        {
            AsAssert.NotNull(list, "AsFindLast: list");
            AsAssert.NotNull(func, "AsFindLast: func");

            for (var i = list.Count() - 1; i >= 0; i--)
            {
                if (func(list.ElementAt(i)))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 查找最后一个符合项
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="list">原集合</param>
        /// <param name="target">符合项</param>
        /// <returns>最后符合项的序列号，如果没有就返回 -1</returns>
        /// <exception cref="ArgumentNullException">输入的参数都不能为 null</exception>
        public static int AsFindLast<T>(this IEnumerable<T> list, T target)
        {
            AsAssert.NotNull(list, "AsFindLast: list");

            bool act(T t)
            {
                return t.Equals(target);
            }

            return list.AsFindLast(act);
        }

        /// <summary>
        /// 在集合中随机取出指定项
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="list">原集合</param>
        /// <param name="num">取出数量</param>
        /// <param name="except">排除的集合</param>
        /// <param name="random">随机数激发工具</param>
        /// <returns>取出的新集合</returns>
        public static List<T> AsRandGet<T>(this IEnumerable<T> list, int num, IEnumerable<T> except = null, Random random = null)
        {
            AsAssert.NotNull(list, "AsRandGet: list");

            var res = new List<T>();

            var listToSelect = list.ToList();

            if (except == null)
                except = new T[0];

            if (random == null)
                random = AsEnumerableExtension.random;

            while (listToSelect.Count() > 0)
            {
                if (res.Count() >= num)
                    break;

                var one = listToSelect[random.Next(0, listToSelect.Count())];

                if (!except.Contains(one))
                {
                    res.Add(one);
                }

                listToSelect.Remove(one);
            }



            return res;
        }

        /// <summary>
        /// 计算可迭代项中某个值的数量
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="list">可迭代项</param>
        /// <param name="target">计数目标</param>
        /// <returns>数量</returns>
        public static int AsCount<T>(this IEnumerable<T> list, T target)
        {
            AsAssert.NotNull(list, "AsCount: list");
            AsAssert.NotNull<object>(target, "AsCount: target");

            int count = 0;

            foreach( var item in list)
            {
                if (target.Equals(item))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
