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
using System.Xml.Linq;

namespace AsTool.Reflection.AsSerialization
{
    internal static class Deserializer
    {
        /// <summary>
        /// 尝试反序列化一个节点
        /// </summary>
        /// <param name="instanceInput">特殊是咯需要提前生成的采用此入口</param>
        /// <param name="type">节点反序列化成的类型</param>
        /// <param name="xmlElement">节点本身</param>
        /// <param name="instance">输出结果</param>
        /// <returns>是否反序列化成功</returns>
        public static bool TryDeserialize(Type type, XmlElement xmlElement, out object instance, object instanceInput = null)
        {
            if (TryDeserializeSimpleValue(type.Name, type, xmlElement, out instance))
                return true;

            if (TryDeserializeSimpleIList(type, xmlElement, out instance))
                return true;

            if (TryDeserializeSimpleIDictionary(type, xmlElement, out instance))
                return true;

            if (TryDeserializeDefault(type, xmlElement, ref instanceInput))
            {
                instance = instanceInput;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 尝试反序列化为简单类型
        /// </summary>
        /// <param name="name">要反序列化的标签</param>
        /// <param name="type">简单类型</param>
        /// <param name="value">输出结果</param>
        /// <param name="xmlElement">要查找的类型</param>
        /// <returns>是否反序列化成功</returns>
        private static bool TryDeserializeSimpleValue(string name, Type type, XmlElement xmlElement, out object value)
        {
            var valueString = xmlElement.GetAttribute(name);
            value = null;

            if (valueString == SerializerCommonUtility.NullString)
                return true;

            if (!SerializerCommonUtility.IsSimpleType(type))
                return false;

            if (string.IsNullOrEmpty(valueString))
                AsAssert.Fatal($"Deserialize Lossing data {name}");

            if (SerializerCommonUtility.TryParseSimpleType(valueString, type, out value))
                return true;

            return false;
        }

        /// <summary>
        /// 尝试反序列化为IList
        /// </summary>
        /// <param name="type">节点的类型</param>
        /// <param name="value">输出结果</param>
        /// <param name="xmlElement">节点本身</param>
        /// <returns>是否反序列化成功</returns>
        /// <exception cref="Exception">反序列化失败触发</exception>
        private static bool TryDeserializeSimpleIList(Type type, XmlElement xmlElement, out object value)
        {
            value = null;

            //如果不是列表类型直接返回
            if (!typeof(IList).IsAssignableFrom(type))
                return false;


            //尝试 List的无参构造
            var valueResult = AsType.GetAsType(type).Init();
            if (valueResult != null)
            {
                var addMethod = SerializerCommonUtility.CatchMethod(valueResult, "Add");
                var itemType = type.GetGenericArguments().FirstOrDefault();

                if (addMethod == null || itemType == null)
                    return false;

                //如果是简单数据，直接加
                if (SerializerCommonUtility.IsSimpleType(itemType))
                    xmlElement.ChildNodes.Cast<XmlElement>().AsForech((e) =>
                    {
                        if (SerializerCommonUtility.TryParseSimpleType(e.InnerText, itemType, out object itemValue))
                            addMethod.Invoke(valueResult, new object[] { itemValue });
                        else
                            throw new Exception("TryDeserializeSimpleIList: Prase failed!");
                    });
                else //如果是复杂数据反序列化了再加
                    xmlElement.Cast<XmlElement>().AsForech((e) =>
                    {
                        if (TryDeserialize(itemType, e, out object item))
                            addMethod.Invoke(valueResult, new object[] { item });
                        else
                            throw new Exception($"TryDeserializeSimpleIList: Deserialize {itemType} failed!");
                    });

                value = valueResult;
                return true;
            }

            //尝试 Arry的有参构造
            valueResult = AsType.GetAsType(type).Init(xmlElement.ChildNodes.Count);

            if (valueResult != null)
            {
                var itemType = type.GetElementType();
                var addMethod = SerializerCommonUtility.CatchMethod(valueResult, "SetValue", itemType, typeof(int));
                var count = 0;

                if (addMethod == null || itemType == null)
                    return false;

                //如果是简单数据，直接加
                if (SerializerCommonUtility.IsSimpleType(itemType))
                    xmlElement.ChildNodes.Cast<XmlElement>().AsForech((e) =>
                    {
                        if (SerializerCommonUtility.TryParseSimpleType(e.InnerText, itemType, out object itemValue))
                            addMethod.Invoke(valueResult, new object[] { itemValue, count++ });
                        else
                            AsAssert.Fatal("TryDeserializeSimpleIList: Prase failed!");
                    });
                else //如果是复杂数据反序列化了再加
                    xmlElement.ChildNodes.Cast<XmlElement>().AsForech((e) =>
                    {
                        if (TryDeserialize(itemType, e, out object item))
                            addMethod.Invoke(valueResult, new object[] { item, count++ });
                        else
                            AsAssert.Fatal("TryDeserializeSimpleIList: Deserialize failed!");
                    });

                value = valueResult;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 反序列化字典
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="xmlElement">节点本身</param>
        /// <param name="value">反序列化结果</param>
        /// <returns>是否反序列化成功</returns>
        private static bool TryDeserializeSimpleIDictionary(Type type, XmlElement xmlElement, out object value)
        {
            value = null;

            //如果不是列表类型直接返回
            if (!typeof(IDictionary).IsAssignableFrom(type))
                return false;

            var newDic = AsType.GetAsType(type).CompelInit();
            var addMethod = typeof(IDictionary).GetMethod("Add");

            Type keyType = type.GetGenericArguments()?.ElementAt(0);

            Type valueType = type.GetGenericArguments()?.ElementAt(1);

            if (newDic == null || addMethod == null || keyType == null || valueType == null)
                return false;

            bool isKeySimple = SerializerCommonUtility.IsSimpleType(keyType);
            bool isValueSimple = SerializerCommonUtility.IsSimpleType(valueType);

            if (isKeySimple && isValueSimple)
                foreach (var item in xmlElement.ChildNodes.Cast<XmlElement>())
                {
                    if (SerializerCommonUtility.TryParseSimpleType(item.GetAttribute(SerializerCommonUtility.DictionaryKeyString), keyType, out object keyValue))
                        if (SerializerCommonUtility.TryParseSimpleType(item.GetAttribute(SerializerCommonUtility.DictionaryValueString), valueType, out object valueValue))
                            addMethod.Invoke(newDic, new object[] { keyValue, valueValue });
                        else
                            AsAssert.Fatal($"Xml Deserialize Dictionary {xmlElement.Name} Failed! try a simpler one please!");
                    else
                        AsAssert.Fatal($"Xml Deserialize Dictionary {xmlElement.Name} Failed! try a simpler one please!");

                }
            else
                foreach (var item in xmlElement.ChildNodes.Cast<XmlElement>())
                {
                    var childList = item.ChildNodes.Cast<XmlElement>();

                    if (TryDeserialize(keyType, isKeySimple ? childList.First(ele => ele.Name == SerializerCommonUtility.DictionaryKeyString) : (XmlElement)childList.First(ele => ele.Name == SerializerCommonUtility.DictionaryKeyString).FirstChild, out object keyValue))
                        if (TryDeserialize(valueType, isValueSimple ? childList.First(ele => ele.Name == SerializerCommonUtility.DictionaryValueString) : (XmlElement)childList.First(ele => ele.Name == SerializerCommonUtility.DictionaryValueString).FirstChild, out object valueValue))
                            addMethod.Invoke(newDic, new object[] { keyValue, valueValue });
                        else
                            AsAssert.Fatal($"Xml Deserialize Dictionary value {xmlElement.Name} Failed! try a simpler one please!");
                    else
                        AsAssert.Fatal($"Xml Deserialize Dictionary key {xmlElement.Name} Failed! try a simpler one please!");
                }

            value = newDic;

            return true;
        }

        /// <summary>
        /// 尝试序列化自定义的结构体和类
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="xmlElement">节点本身</param>
        /// <param name="value">反序列化结果</param>
        /// <returns>是否反序列化成功</returns>
        /// <exception cref="Exception">反序列化失败触发</exception>
        private static bool TryDeserializeDefault(Type type, XmlElement xmlElement, ref object value)
        {

            var newItem = value ?? AsType.GetAsType(type).CompelInit();

            var taskList = new List<Task>();

            if (newItem == null)
            {
                AsLog.Fatal($"Deserialize Build type {type} failed maybe is a abstruct or static class?");

                return false;
            }

            bool NeedSerialize(MemberInfo info)
            {
                return SerializerCommonUtility.NeedSerilize(info);
            }

            var xmlList = xmlElement.Cast<XmlElement>();

            foreach (var info in AsType.GetAsType(type).FieldAndPropertyMembers.Where((info) => !info.IsAutomaticallyGenerated && NeedSerialize(info.MemberInfo)))
            {
                switch (info.MemberType)
                {
                    case MemberTypes.Property:
                        DeserializeProperties((PropertyInfo)info.MemberInfo);
                        break;

                    case MemberTypes.Field:
                        DeserializeFields((FieldInfo)info.MemberInfo);
                        break;

                    default:
                        AsAssert.Fatal($"get a unexpected memberInfo {info.MemberInfo.Name}");
                        break;
                }

            }

            void DeserializeProperties(PropertyInfo info)
            {
                if(!info.CanWrite)
                    return;

                if (SerializerCommonUtility.IsSimpleType(info.PropertyType))
                {
                    if (xmlElement.GetAttribute(info.Name) == "NULL")
                        return;

                    if (SerializerCommonUtility.TryParseSimpleType(xmlElement.GetAttribute(info.Name),info.PropertyType, out object propertyValue))
                        info.SetValue(newItem, propertyValue);
                    else
                        AsAssert.Fatal($"TryDeserializeDefault: Prase {info.Name} failed!");
                }
                else
                {
                    var targetXml = xmlList.FirstOrDefault(ele => ele.Name == info.Name);

                    if (targetXml == null)
                    {
                         if (info.PropertyType.IsValueType)
                            info.SetValue(newItem, AsType.GetAsType(info.PropertyType).CompelInit());
                         else
                            info.SetValue(newItem, null);
                    }
                    else
                    {
                        if (TryDeserialize(info.PropertyType, targetXml, out object propertyValue))
                            info.SetValue(newItem, propertyValue);
                        else
                            AsAssert.Fatal($"TryDeserializeDefault: TryDeserialize {info.Name} failed!");
                    }
                }
            }

            void DeserializeFields(FieldInfo info)
            {
                if (/*info.IsInitOnly || */info.IsLiteral)
                    return;

                if (SerializerCommonUtility.IsSimpleType(info.FieldType))
                {
                    if (xmlElement.GetAttribute(info.Name) == "NULL")
                        return;

                    if (SerializerCommonUtility.TryParseSimpleType(xmlElement.GetAttribute(info.Name), info.FieldType, out object propertyValue))
                        info.SetValue(newItem, propertyValue);
                    else
                        AsAssert.Fatal($"TryDeserializeDefault: Prase {info.Name} failed!");
                }
                else
                {
                    var targetXml = xmlList.FirstOrDefault(ele => ele.Name == info.Name);

                    if (targetXml == null)
                    {
                        if (info.FieldType.IsValueType)
                            info.SetValue(newItem, AsType.GetAsType(info.FieldType).CompelInit());
                        else
                            info.SetValue(newItem, null);
                    }
                    else
                    {
                        if (TryDeserialize(info.FieldType, targetXml, out object propertyValue))
                            info.SetValue(newItem, propertyValue);
                        else
                            AsAssert.Fatal($"TryDeserializeDefault: TryDeserialize {info.Name} failed!");
                    }
                }


            }

            //Task.WaitAll(taskList.ToArray());

            value = newItem;
            return true;
        }
    }
}
