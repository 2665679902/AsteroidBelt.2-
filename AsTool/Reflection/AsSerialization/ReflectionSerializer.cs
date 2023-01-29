using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AsTool.Reflection.AsSerialization
{

    internal class ReflectionSerializer
    {
        /// <summary>
        /// 采取保守方案序列化简单类型的字段和属性，不保证基类的完整构建
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>序列化结果</returns>
        public static XmlElement SerializeToNode(object input)
        {
            XmlElement GetDefaultElement()
            {
                var doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", ""));
                var elementtNode = doc.CreateElement("AsData");
                doc.AppendChild(elementtNode);
                return elementtNode;
            }

            var type = input.GetType();

            var rootNode = GetDefaultElement();

            if(Serializer.TrySerialize(type.Name, input, rootNode.OwnerDocument, ref rootNode))
                return rootNode;
            else
                return null;
        }

        /// <summary>
        /// 尝试将一个XmlElement反序列化为一个类型
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="type">目标类型</param>
        /// <returns>反序列化结果</returns>
        public static object DeserializeFromNode(XmlElement root, Type type)
        {
            if(Deserializer.TryDeserialize(type, (XmlElement)root.ChildNodes[0], out object result, AsType.GetAsType(type).CompelInit()))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
