using AsTool.Assert;
using AsTool.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Reflection.AsSerialization
{
    internal static class SerializerCommonUtility
    {

        public const string NullString = "NULL";

        public const string ListItemString = "item";

        public const string DictionaryPairString = "pair";

        public const string DictionaryKeyString = "key";

        public const string DictionaryValueString = "value";

        /// <summary>
        /// 要序列化的目标
        /// </summary>
        public static BindingFlags All { get => BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance; }

        /// <summary>
        /// 获取所有的成员信息
        /// </summary>
        /// <param name="input">要获取的对象</param>
        /// <returns>获取结果</returns>
        public static IEnumerable<AsType.AsMemberInfo> GetValues(in object input)
        {
            var info = AsType.GetAsType(input.GetType()).FieldAndPropertyMembers;

            return info;
        }

        /// <summary>
        /// 判断 type 是否是一个可以简单地转为字符串而不丢失信息的类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>是否是简单类型</returns>
        public static bool IsSimpleType(Type type)
        {
            AsAssert.NotNull(type, "SerializerCommonUtility.IsSimpleType get null");

            if (type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null) != null)
                return true;
            if (type.IsEnum)
                return true;
            if (type == typeof(string))
                return true;

            return false;
        }

        /// <summary>
        /// 尝试反序列化简单类型
        /// </summary>
        /// <param name="str">反序列化的源字符串</param>
        /// <param name="type">目标类型</param>
        /// <param name="result">结果，失败为null</param>
        /// <returns>失败返回 false ，成功返回 true</returns>
        /// <exception cref="ArgumentNullException">参数不可为 null</exception>
        public static bool TryParseSimpleType(string str, Type type, out object result)
        {
            AsAssert.NotNull(type, "SerializerCommonUtility.TryParseSimpleType get null type");

            result = null;

            //判断string
            if (type == typeof(string))
            {
                result = str;
                return true;
            }

            if (string.IsNullOrEmpty(str) || !IsSimpleType(type))
                return false;

            MethodInfo methodInfo;

            try
            {
                //判断简单类型
                methodInfo = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);

                if (methodInfo != null)
                {
                    result = methodInfo.Invoke(null, new object[] { str });
                    return true;
                }

                //判断Enum
                if (type.IsEnum)
                {
                    result = Enum.Parse(type, str);
                    return true;
                }
            }
            catch
            { 
                return false; 
            }

            return false;
        }

        /// <summary>
        /// 尝试获取方法
        /// </summary>
        /// <param name="o">要获取方法的实例</param>
        /// <param name="name">方法的名字</param>
        /// <param name="arguments">方法的参数</param>
        /// <returns>方法</returns>
        /// <exception cref="ArgumentNullException">参数不可为 null 或 空</exception>
        public static MethodInfo CatchMethod(object o, string name, params Type[] arguments)
        {
            if (o == null || string.IsNullOrEmpty(name))
                throw new ArgumentNullException("AsObject CatchMethod: str , name");

            if (!arguments.Any())
                return o.GetType().GetMethod(name, All);
            else
            {
                return o.GetType().GetMethod(name, All, null, arguments, null);
            }
        }

        /// <summary>
        /// 尝试获取基元类型或字符串的默认值
        /// </summary>
        /// <param name="type">要获取的基元类型或字符串</param>
        /// <returns>默认值（不是就返回null）</returns>
        /// <exception cref="ArgumentNullException">参数不可为 null</exception>
        public static object GetDefaultPrimitiveValue(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!type.IsPrimitive)
                return null;

            if (type == typeof(bool))
                return default(bool);

            if (type == typeof(IntPtr))
                return default(IntPtr);

            if (type == typeof(UIntPtr))
                return default(UIntPtr);

            if (type == typeof(char))
                return default(char);

            return 0;
        }

        /// <summary>
        /// 防止序列化的标签
        /// </summary>
        public static Type DontSerializeAttribute { get; set; } = typeof(AsNoSerializeAttribute);

        /// <summary>
        /// 判断字段、属性是否需要序列化
        /// </summary>
        /// <param name="info">要判断的字段</param>
        /// <returns>是否应被序列化</returns>
        public static bool NeedSerilize(MemberInfo info)
        {
            return !info.IsDefined(DontSerializeAttribute, true);
        }
    }

    /// <summary>
    /// 阻止此 字段/属性 被序列化, 可继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AsNoSerializeAttribute : Attribute
    {

    }
}
