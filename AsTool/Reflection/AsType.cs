using AsTool.Assert;
using AsTool.Common.Extension;
using AsTool.Reflection.AsSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AsTool.Reflection
{
    /// <summary>
    /// 反射工具类
    /// </summary>
    public sealed class AsType
    {
        /// <summary>
        /// 搜索所有的对象
        /// </summary>
        private const BindingFlags All = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// 单次储存所有生成的反射类
        /// </summary>
        private static readonly Dictionary<Type,AsType> _asTypes = new Dictionary<Type, AsType>();

        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// 储存的反射类的数量
        /// </summary>
        public static int Count { get { return _asTypes.Count; } }

        /// <summary>
        /// 清空储存的反射类
        /// </summary>
        public static void Clear()
        {
            _asTypes.Clear();

            GC.Collect();
        }

        /// <summary>
        /// 获取反射工具，线程安全
        /// </summary>
        /// <param name="type">要获取工具的对象类</param>
        /// <returns>返回工具类</returns>
        public static AsType GetAsType(Type type)
        {
            AsAssert.NotNull(type, "GetAsType get null type");

            lock (_lock)
            {
                if(!_asTypes.TryGetValue(type, out AsType result))
                {
                    result = new AsType(type);

                    _asTypes.Add(type, result);
                }

                return result;
            }
        }

        /// <summary>
        /// 序列化此类
        /// </summary>
        /// <param name="o">要序列化的类</param>
        /// <returns>序列化结果</returns>
        public static XmlElement AsSerialize(object o)
        {
            AsAssert.NotNull(o, "o can not be null");

            return ReflectionSerializer.SerializeToNode(o);
        }

        /// <summary>
        /// 反序列化根节点为某一类
        /// </summary>
        /// <param name="root">要反序列化的节点</param>
        /// <param name="type">反序列化目标</param>
        /// <returns>反序列化结果</returns>
        public static object AsDeserialize(XmlElement root, Type type)
        {
            AsAssert.NotNull(root, "root can not be null");

            AsAssert.NotNull(type, "type can not be null");

            return ReflectionSerializer.DeserializeFromNode(root, type);
        }

        /// <summary>
        /// 从目标范围中通过属性类型查找成员
        /// </summary>
        /// <param name="assembly">从该程序集中查找</param>
        /// <param name="attributeType">属性类型</param>
        /// <returns>查找结果</returns>
        public static List<AsMemberInfo> GetMemberInfoFrom(Assembly assembly, Type attributeType)
        {
            var result = new List<AsMemberInfo>();

            foreach (var type in assembly
                .GetTypes()
                .Select(GetAsType)
                .Where((asType) => asType.MembersAttributeTypes.Contains(attributeType)))
            {
                foreach (var info in type.Members.Where((m) => m.TryGetCustomAttribute(attributeType, out _)))
                {
                    result.Add(info);
                }
            }

            return result;
        }

        /// <summary>
        /// 从目标范围中通过属性类型查找成员
        /// </summary>
        /// <param name="assemblys">从这些程序集中查找</param>
        /// <param name="attributeType">属性类型</param>
        /// <returns>查找结果</returns>
        public static List<AsMemberInfo> GetMemberInfoFrom(IEnumerable<Assembly> assemblys, Type attributeType)
        {
            var result = new List<AsMemberInfo>();

            foreach(var assembly in assemblys)
            {
                var temp = GetMemberInfoFrom(assembly, attributeType);
                result = result.Concat(temp).ToList();
            }

            return result;
        }

        /// <summary>
        /// 通过基类寻找匹配任意基类(不包括基类本身)的类型
        /// </summary>
        /// <param name="assembly">要查找的程序集</param>
        /// <param name="baseTypes">要匹配的基类</param>
        /// <returns>匹配结果</returns>
        public static List<AsType> GetAnyAsTypesFrom(Assembly assembly, Type[] baseTypes)
        {
            var result = new List<AsType>();

            foreach (var type in assembly.GetTypes())
            {
                foreach(var baseType in baseTypes)
                {
                    if(baseType.IsAssignableFrom(type) && baseType != type)
                    {
                        result.Add(GetAsType(type));
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 通过基类寻找匹配任意基类(不包括基类本身)的类型
        /// </summary>
        /// <param name="assemblys">要查找的程序集</param>
        /// <param name="baseTypes">要匹配的基类</param>
        /// <returns>匹配结果</returns>
        public static List<AsType> GetAnyAsTypesFrom(IEnumerable<Assembly> assemblys, Type[] baseTypes)
        {
            var result = new List<AsType>();

            foreach(var assembly in assemblys)
            {
                result.AddRange(GetAnyAsTypesFrom(assembly, baseTypes));
            }

            return result;
        }

        /// <summary>
        /// 构建并保存类
        /// </summary>
        /// <param name="type">要构建反射工具的类</param>
        private AsType(Type type)
        {
            _type = type;

            //放弃不正常的类型
            if (type.IsGenericType && type.GenericTypeArguments.Length == 0)
                return;
            
            _compelInitFunc = ExpressionConstructor.GetCompelConstructFunction(type);

            BuildMemberInfo();
        }

        /// <summary>
        /// 构建反射工具的对象类
        /// </summary>
        private readonly Type _type;

        #region Init

        /// <summary>
        /// 强制构建目标函数的方法
        /// </summary>
        private readonly Func<object> _compelInitFunc;

        /// <summary>
        /// 强制获取目标类型的实体，如果构造失败返回null
        /// </summary>
        /// <returns>实例化的对象</returns>
        public object CompelInit()
        {
            return _compelInitFunc?.Invoke();
        }

        /// <summary>
        /// 查找对应参数的构造方案
        /// </summary>
        /// <param name="arguments">传入的参数</param>
        /// <returns>实例化的对象，未找到构造函数返回null</returns>
        public object Init(params object[] arguments)
        {
            var result = _type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    arguments.Select((o) => o.GetType()).ToArray(),
                    null);

            if (result == null)
                return null;

            return result.Invoke(arguments);
        }

        #endregion

        #region MemberInfo

        /// <summary>
        /// 构建值展示
        /// </summary>
        private void BuildMemberInfo()
        {
            _members = new HashSet<AsMemberInfo>();

            _membersAttributeTypes = new HashSet<Type>();

            foreach (var field in _type.GetFields(All))
            {
                foreach(var type in field.GetCustomAttributes(true))
                {
                    _membersAttributeTypes.Add(type.GetType());
                }


                Action<object, object> setValueFunc;

                if (field.IsLiteral)
                {
                    setValueFunc = null;
                }
                else if (field.IsStatic)
                {
                    setValueFunc = field.SetValue;
                }
                else
                {
                    //尝试用表达式树来构建对值的访问

                    var input = Expression.Parameter(typeof(object), "input");

                    var instance = Expression.Parameter(typeof(object), "instance");

                    try
                    {
                        BinaryExpression value = Expression.Assign(Expression.Field(Expression.Convert(instance, _type), field), Expression.Convert(input, field.FieldType));

                        var result = Expression.Lambda<Action<object, object>>(value, instance, input);

                        setValueFunc = result.Compile();
                    }
                    catch
                    {
                        //对于泛型结果上述方法不可用 有些时候运行时会拒绝上述访问行为
                        setValueFunc = field.SetValue;
                    }
                    
                }

                Func<object, object> getValueFunc;


                if (field.IsStatic)
                {
                    getValueFunc = field.GetValue;
                }
                else
                {
                    //尝试用表达式树来构建对值的访问
                    var instance = Expression.Parameter(typeof(object), "instance");

                    try
                    {
                        var value = Expression.Convert(Expression.Field(Expression.Convert(instance, _type), field), typeof(object));

                        var result = Expression.Lambda<Func<object, object>>(value, instance);

                        getValueFunc = result.Compile();
                    }
                    catch
                    {
                        //对于泛型结果上述方法不可用
                        getValueFunc = field.GetValue;
                    }
                }
                
                _members.Add(
                    new AsMemberInfo(
                    field,
                    getValueFunc,
                    setValueFunc,
                    MemberTypes.Field,
                    field.IsStatic
                    ));
            }

            foreach(var property in _type.GetProperties(All))
            {

                foreach (var type in property.GetCustomAttributes(true))
                {
                    _membersAttributeTypes.Add(type.GetType());
                }

                Action<object, object> setValueFunc = null;

                if (property.CanWrite)
                {
                    //尝试用表达式树来构建对值的访问

                    var input = Expression.Parameter(typeof(object), "input");

                    var instance = Expression.Parameter(typeof(object), "instance");

                    try
                    {
                        var value = Expression.Assign(Expression.Property(Expression.Convert(instance, _type), property), Expression.Convert(input, property.PropertyType));

                        var result = Expression.Lambda<Action<object, object>>(value, instance, input);

                        setValueFunc = result.Compile();
                    }
                    catch
                    {
                        //对于泛型结果上述方法不可用 有些时候运行时会拒绝上述访问行为

                        setValueFunc = property.SetValue;
                    }
                }

                Func<object, object> getValueFunc = null;

                if (property.CanRead)
                {
                    //尝试用表达式树来构建对值的访问

                    var instance = Expression.Parameter(typeof(object), "instance");

                    

                    try
                    {
                        var value = Expression.Convert(Expression.Property(Expression.Convert(instance, _type), property), typeof(object));

                        var result = Expression.Lambda<Func<object, object>>(value, instance);

                        getValueFunc = result.Compile();
                    }
                    catch
                    {
                        //对于泛型结果上述方法不可用 有些时候运行时会拒绝上述访问行为
                        getValueFunc = property.GetValue;
                    }
                }

                _members.Add(
                    new AsMemberInfo(
                    property,
                    getValueFunc,
                    setValueFunc,
                    MemberTypes.Property,
                    (property.SetMethod?.IsStatic ?? false) || (property.GetMethod?.IsStatic ?? false)
                    ));
            }

            foreach(var method in _type.GetMethods(All))
            {
                foreach (var type in method.GetCustomAttributes(true))
                {
                    _membersAttributeTypes.Add(type.GetType());
                }

                _members.Add(
                    new AsMemberInfo(
                        method,
                        null,
                        null,
                        MemberTypes.Method,
                        method.IsStatic
                    ));
            }

            _fieldAndPropertyMembers = _members.Where((info) => info.MemberType== MemberTypes.Field || info.MemberType == MemberTypes.Property);
        }

        /// <summary>
        /// 所有成员访问
        /// </summary>
        HashSet<AsMemberInfo> _members = new HashSet<AsMemberInfo>();

        /// <summary>
        /// 所有成员访问
        /// </summary>
        public IEnumerable<AsMemberInfo> Members { get { return _members; } }

        /// <summary>
        /// 所有字段和属性成员
        /// </summary>
        private IEnumerable<AsMemberInfo> _fieldAndPropertyMembers = new AsMemberInfo[0];

        /// <summary>
        /// 所有字段和属性成员
        /// </summary>
        public IEnumerable<AsMemberInfo> FieldAndPropertyMembers { get { return _fieldAndPropertyMembers; } }

        /// <summary>
        /// 本类型的成员包含的所有属性类型
        /// </summary>
        public HashSet<Type> _membersAttributeTypes = new HashSet<Type>();

        /// <summary>
        /// 本类型的成员包含的所有属性类型
        /// </summary>
        public IEnumerable<Type> MembersAttributeTypes { get => _membersAttributeTypes; }

        /// <summary>
        /// 显示一个值并取得其获取和设置方法
        /// </summary>
        public class AsMemberInfo
        {
            /// <summary>
            /// 反射对象
            /// </summary>
            public readonly MemberInfo MemberInfo;

            /// <summary>
            /// 获取该值的委托
            /// </summary>
            private readonly Func<object, object> GetValueFunc;

            /// <summary>
            /// 设置该值的委托
            /// </summary>
            private readonly Action<object, object> SetValueFunc;

            /// <summary>
            /// 该成员的类型
            /// </summary>
            public readonly MemberTypes MemberType;

            /// <summary>
            /// 本实例是否是静态的
            /// </summary>
            public readonly bool IsStatic;

            /// <summary>
            /// 返回本实例是否是属性
            /// </summary>
            public bool IsProperty { get => MemberType == MemberTypes.Property; }

            /// <summary>
            /// 返回本实例是否是字段
            /// </summary>
            public bool IsField { get => MemberType == MemberTypes.Field; }

            /// <summary>
            /// 返回本实例是否是函数
            /// </summary>
            public bool IsMethod { get => MemberType == MemberTypes.Method; }

            /// <summary>
            /// 该成员是否由系统自动生成
            /// </summary>
            public bool IsAutomaticallyGenerated
            {
                get => MemberInfo.Name.StartsWith("<");
            }

            /// <summary>
            /// 该值是否可以设置
            /// </summary>
            public bool Setable { get => !(SetValueFunc is null); }

            /// <summary>
            /// 该值是否可以获取
            /// </summary>
            public bool Getable { get => !(GetValueFunc is null); }

            /// <summary>
            /// 构建一个显示
            /// </summary>
            /// <param name="memberInfo">反射对象</param>
            /// <param name="getValueFunc">获取该值的委托</param>
            /// <param name="setValueFunc">设置该值的委托</param>
            /// <param name="memberType">该成员的类型</param>
            /// <param name="isStatic">本实例是否是静态的</param>
            public AsMemberInfo(MemberInfo memberInfo, Func<object, object> getValueFunc, Action<object, object> setValueFunc, MemberTypes memberType, bool isStatic)
            {
                MemberInfo = memberInfo;
                GetValueFunc = getValueFunc;
                SetValueFunc = setValueFunc;
                MemberType = memberType;
                IsStatic = isStatic;
            }

            /// <summary>
            /// 获取该值
            /// </summary>
            /// <param name="instance">要获取的实例, 如果是静态方法则设为 null</param>
            /// <returns>目标值</returns>
            public object GetValue(object instance)
            {
                return GetValueFunc?.Invoke(instance);
            }

            /// <summary>
            /// 设置该值
            /// </summary>
            /// <param name="instance">要设置的实例, 如果是静态方法则设为 null</param>
            /// <param name="value">要设置的值</param>
            public void SetValue(object instance, object value)
            {
                SetValueFunc?.Invoke(instance, value);
            }

            /// <summary>
            /// 如果该项是一个方法, 则可以触发该方法, 否则返回 null
            /// </summary>
            /// <param name="instance">要触发方法的实例， 如果是静态函数则输入null</param>
            /// <param name="Parameter">参数</param>
            /// <returns>方法结果, 如果触发失败或则是无返回值的方法则返回 null </returns>
            public object InvokeMethod(object instance, params object[] Parameter)
            {
                object result = null ;

                if(MemberInfo is MethodInfo info)
                {
                    result = info.Invoke(instance, Parameter);
                }

                return result;
            }

            /// <summary>
            /// 本成员是否包含目标属性
            /// </summary>
            /// <param name="attributeType">属性</param>
            /// <param name="attribute">查找到的目标属性, 如果没有则返回null</param>
            /// <returns>判断结果</returns>
            public bool TryGetCustomAttribute(Type attributeType, out Attribute attribute)
            {
                attribute = Attribute.GetCustomAttribute(MemberInfo, attributeType);

                return attribute != null;
            }
        }

        #endregion
    }
}
