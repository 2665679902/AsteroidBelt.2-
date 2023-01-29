using AsTool.Assert;
using AsTool.Common.Extension;
using AsTool.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AsTool.Reflection.AsSerialization
{
    internal class Serializer
    {
        /// <summary>
        /// 查找数据类型
        /// </summary>
        private static BindingFlags TargetFlag { get => SerializerCommonUtility.All; }

        /// <summary>
        /// 尝试序列化入口
        /// </summary>
        /// <param name="name">下级节点的名字</param>
        /// <param name="value">节点的值</param>
        /// <param name="document">要写入的文档</param>
        /// <param name="xmlElement">根节点</param>
        /// <returns>是否序列化成功</returns>
        public static bool TrySerialize(string name, object value, XmlDocument document, ref XmlElement xmlElement)
        {
            if (name.StartsWith("<"))
                return true;

            if (TrySpecialSerialize(name, value, document, ref xmlElement))
                return true;

            if (TrySerializeDefault(name, value, document, ref xmlElement))
                return true;

            return false;
        }

        /// <summary>
        /// 尝试序列化列表、字典、数组、简单类型
        /// </summary>
        /// <param name="name">下级节点的名字</param>
        /// <param name="value">节点的值</param>
        /// <param name="document">要写入的文档</param>
        /// <param name="xmlElement">根节点</param>
        /// <returns>是否序列化成功</returns>
        private static bool TrySpecialSerialize(string name, object value, XmlDocument document, ref XmlElement xmlElement)
        {
            if (TrySerializeSimpleValue(name, value, ref xmlElement))
                return true;


            if (TrySerializeIList(name, value, document, ref xmlElement))
                return true;

            if (TrySerializeIDictionary(name, value, document, ref xmlElement))
                return true;

            return false;
        }

        /// <summary>
        /// 尝试序列化简单类型
        /// </summary>
        /// <param name="name">下级节点的名字</param>
        /// <param name="value">节点的值</param>
        /// <param name="xmlElement">根节点</param>
        /// <returns>是否序列化成功</returns>
        private static bool TrySerializeSimpleValue(string name, object value, ref XmlElement xmlElement)
        {
            if (value == null)
            {
                xmlElement.SetAttribute(name, SerializerCommonUtility.NullString);
                return true;
            }

            var type = value.GetType();

            //不是简单类型直接返回
            if (!SerializerCommonUtility.IsSimpleType(type))
                return false;

            xmlElement.SetAttribute(name, value.ToString());

            return true;
        }

        /// <summary>
        /// 尝试序列化列表、数组
        /// </summary>
        /// <param name="name">下级节点的名字</param>
        /// <param name="value">节点的值</param>
        /// <param name="document">要写入的文档</param>
        /// <param name="xmlElement">根节点</param>
        /// <returns>是否序列化成功</returns>
        private static bool TrySerializeIList(string name, object value, XmlDocument document, ref XmlElement xmlElement)
        {
            //如果不是列表类型直接返回
            if (!typeof(IList).IsAssignableFrom(value.GetType()))
                return false;

            //查找内层函数类型
            Type type;

            if (value.GetType().IsGenericType)
                type = value.GetType().GetGenericArguments()?.ElementAt(0);
            else
                type = value.GetType().GetElementType();

            var listNode = document.CreateElement(name);

            xmlElement.AppendChild(listNode);

            //简单类型直接子项赋值
            if (SerializerCommonUtility.IsSimpleType(type))
            {
                foreach (var item in (IList)value)
                {
                    var itemNode = document.CreateElement(SerializerCommonUtility.ListItemString);
                    itemNode.InnerText = item.ToString();
                    listNode.AppendChild(itemNode);
                }

                return true;
            }

            //遍历子项
            foreach (var item in (IList)value)
            {
                //尝试递归序列化所有子项，序列化失败返回false
                if (!TrySerialize("item", item, document, ref listNode))
                    AsAssert.Fatal($"Xml Serialize IList {name} Failed! try a simpler one please!");
            }

            return true;
        }

        /// <summary>
        /// 尝试序列化字典
        /// </summary>
        /// <param name="name">下级节点的名字</param>
        /// <param name="value">节点的值</param>
        /// <param name="document">要写入的文档</param>
        /// <param name="xmlElement">根节点</param>
        /// <returns>是否序列化成功</returns>
        /// <exception cref="Exception">序列化失败事件</exception>
        private static bool TrySerializeIDictionary(string name, object value, XmlDocument document, ref XmlElement xmlElement)
        {
            //如果不是列表类型直接返回
            if (!typeof(IDictionary).IsAssignableFrom(value.GetType()))
                return false;

            //查找内层函数类型
            Type keyType = value.GetType().GetGenericArguments()?.ElementAt(0);

            Type valueType = value.GetType().GetGenericArguments()?.ElementAt(1);

            var rootNode = document.CreateElement(name);
            xmlElement.AppendChild(rootNode);

            //简单类型直接子项赋值
            if (SerializerCommonUtility.IsSimpleType(keyType) && SerializerCommonUtility.IsSimpleType(valueType))
            {
                foreach (DictionaryEntry item in (IDictionary)value)
                {
                    var itemNode = document.CreateElement(SerializerCommonUtility.DictionaryPairString);
                    itemNode.SetAttribute(SerializerCommonUtility.DictionaryKeyString, item.Key.ToString());
                    itemNode.SetAttribute(SerializerCommonUtility.DictionaryValueString, item.Value.ToString());
                    rootNode.AppendChild(itemNode);
                }

                return true;
            }


            //遍历子项
            foreach (DictionaryEntry item in ((IDictionary)value))
            {
                var itemNode = document.CreateElement(SerializerCommonUtility.DictionaryPairString);
                var keyNode = document.CreateElement(SerializerCommonUtility.DictionaryKeyString);
                var valueNode = document.CreateElement(SerializerCommonUtility.DictionaryValueString);

                itemNode.AppendChild(keyNode);
                itemNode.AppendChild(valueNode);
                rootNode.AppendChild(itemNode);

                //尝试递归序列化所有子项，序列化失败返回false
                if (TrySerialize(item.Key.GetType().Name, item.Key, document, ref keyNode))
                    if (TrySerialize(item.Value.GetType().Name, item.Value, document, ref valueNode))
                    {
                        continue;
                    }
                    else
                        AsAssert.Fatal($"Xml Serialize Dictionary {name} Failed! try a simpler one please!");
                else
                    AsAssert.Fatal($"Xml Serialize Dictionary {name} Failed! try a simpler one please!");
            }

            return true;
        }

        /// <summary>
        /// 尝试序列化自定义的结构体和类
        /// </summary>
        /// <param name="name">下级节点的名字</param>
        /// <param name="value">节点的值</param>
        /// <param name="document">要写入的文档</param>
        /// <param name="xmlElement">根节点</param>
        /// <returns>是否序列化成功</returns>
        /// <exception cref="Exception">序列化失败事件</exception>
        private static bool TrySerializeDefault(string name, object value, XmlDocument document, ref XmlElement xmlElement)
        {
            bool NeedSerialize(MemberInfo info)
            {
                return SerializerCommonUtility.NeedSerilize(info);
            }

            var classNode = document.CreateElement(name);

            xmlElement.AppendChild(classNode);

            foreach (var pair in
                SerializerCommonUtility.GetValues(value)
                .Where(info => info.Getable && NeedSerialize(info.MemberInfo))
                .Select(info => new KeyValuePair<string,object>(info.MemberInfo.Name, info.GetValue(value)))
                )
            {
                if (!TrySerialize(pair.Key, pair.Value, document, ref classNode))
                    AsAssert.Fatal($"Xml Serialize Class/Struct {name} value {pair.Key} Failed! try a simpler one please!");

            }

            return true;
        }
    }
}
