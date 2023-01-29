using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.Reflection
{
    internal static class ExpressionConstructor
    {
        static readonly BindingFlags _all = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// 构建强制构造表达式
        /// </summary>
        /// <param name="type">要构建的类型</param>
        /// <returns>如果找到表达式则返回，否则返回null</returns>
        public static Expression GetCompelConstructExpression(Type type)
        {
            //遍历调用栈 构建构造方法
            Expression InnerConstructor(Type Ttype, HashSet<Type> stack)
            {
                //如果是字符串或者空值，直接返回空
                if (Ttype == typeof(string))
                    return Expression.Constant(string.Empty);

                //如果是值类型，直接返回
                if (Ttype.IsValueType || Ttype.IsArray)
                    return Expression.Default(Ttype);

                //如果无参构造存在 直接构建表达式
                if (Ttype.GetConstructor(_all, null, Type.EmptyTypes, null) != null)
                    return Expression.New(Ttype);

                if (!Ttype.IsSecuritySafeCritical)
                    return null;

                foreach (var method in Ttype.GetConstructors(_all).OrderBy((c) => c.GetParameters().Length))
                {
                    var paraList = new Expression[method.GetParameters().Length];

                    int count = 0;

                    foreach (var parameter in method.GetParameters())
                    {
                        //检查调用栈，防止爆栈
                        if (stack.Contains(parameter.ParameterType))
                            return null;

                        stack.Add(parameter.ParameterType);

                        if (parameter.HasDefaultValue)
                            paraList[count++] = Expression.Convert(Expression.Constant(parameter.DefaultValue), parameter.ParameterType);
                        else
                            paraList[count++] = InnerConstructor(parameter.ParameterType, stack);

                        stack.Remove(parameter.ParameterType);
                    }

                    try
                    {
                        var newFunc = Expression.New(method, paraList);
                        var obj = Expression.TypeAs(newFunc, typeof(object));
                        
                        //尝试构建一次，没有问题就返回
                        var resFunc = Expression.Lambda<Func<object>>(obj)?.Compile();
                        var tryRes = resFunc?.Invoke();

                        if (tryRes == null)
                            continue;

                        if(tryRes is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                    catch
                    {
                        continue;
                    }

                    return Expression.New(method, paraList);
                }

                return null;
            }

            if (type.IsAbstract)
                return null;

            return InnerConstructor(type, new HashSet<Type>());
        }

        /// <summary>
        /// 构建强制构造函数
        /// </summary>
        /// <typeparam name="T">要构建的类型</typeparam>
        /// <returns>如果找到可用的构造函数则返回，否则返回null</returns>
        public static Func<T> GetCompelConstructFunction<T>()
        {
            var result = GetCompelConstructFunction(typeof(T));

            if (result != null)
                return () => (T)result.Invoke();

            return null;
        }

        /// <summary>
        /// 构建强制构造函数
        /// </summary>
        /// <param name="type">要构建的类型</param>
        /// <returns>如果找到可用的构造函数则返回，否则返回null</returns>
        public static Func<object> GetCompelConstructFunction(Type type)
        {
            var result = GetCompelConstructExpression(type);
            
            try
            {
                if (result != null)
                    return Expression.Lambda<Func<object>>(Expression.TypeAs(result, typeof(object))).Compile();
            }
            catch
            { 

            }
           

            return null;
        }
    }
}
