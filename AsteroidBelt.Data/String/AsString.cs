using AsTool.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsTool.Extension;
using AsTool.Common.Extension;

namespace AsteroidBelt.Data.String
{
    /// <summary>
    /// 一个封装的string, 默认进行翻译行为
    /// </summary>
    public class AsString
    {
        /// <summary>
        /// 所有语言类型
        /// </summary>
        public enum Language
        {
            /// <summary>
            /// 中文
            /// </summary>
            Chinese,
            /// <summary>
            /// 日文
            /// </summary>
            Japanese,
            /// <summary>
            /// 韩文
            /// </summary>
            Korean,
            /// <summary>
            /// 俄文
            /// </summary>
            Russian,
            /// <summary>
            /// 泰文
            /// </summary>
            Thai,
            /// <summary>
            /// 阿拉伯文
            /// </summary>
            Arabic,
            /// <summary>
            /// 希伯来文
            /// </summary>
            Hebrew,
            /// <summary>
            /// 未指定(英文?)
            /// </summary>
            Unspecified
        }

        /// <summary>
        /// 保存内部的翻译信息
        /// </summary>
        private static readonly Dictionary<string, string> _translateDictionary = new Dictionary<string, string>();

        /// <summary>
        /// 添加翻译信息
        /// </summary>
        /// <param name="pairs">要添加的翻译信息</param>
        public static void AddTranslate(Dictionary<string, string> pairs)
        {
            foreach (var pair in pairs)
            {
                _translateDictionary[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// 用于测试标记要翻译文本的函数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string _(string str) => str;

        /// <summary>
        /// 测试
        /// </summary>
        private static readonly AsString testString = _("test");

        /// <summary>
        /// 内部封装的string对象
        /// </summary>
        private string key;

        /// <summary>
        /// 允许无参构建
        /// </summary>
        public AsString() { }

        /// <summary>
        /// 允许在构建时传入封装值
        /// </summary>
        /// <param name="key">封装值</param>
        public AsString(string key) { this.key = key; }

        /// <summary>
        /// 允许设置内部封装的值
        /// </summary>
        public string Value { get { return key; } set { key = value; } }

        /// <summary>
        /// 尝试翻译， 翻译失败会返回原值
        /// </summary>
        /// <returns>翻译结果</returns>
        public string Translate()
        {
            if(_translateDictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return key;
        }

        /// <summary>
        /// 尝试进行翻译
        /// </summary>
        /// <param name="values">会使用这些值进行<see cref="string.Format(string,object[])"/></param>
        /// <returns>翻译结果</returns>
        public string Translate(params object[] values)
        {
            return string.Format(Translate(), values);
        }

        /// <summary>
        /// 加载对应的翻译文件
        /// </summary>
        /// <param name="language">语言类型</param>
        public static void LoadFromPo(Language language)
        {
            switch(language)
            {
                case Language.Unspecified:
                    return;

                case Language.Chinese:
                    FormatePoToDic("zh");
                    AsLog.Info($"中文加载完成 Load Chinese finished, Test -> {testString}");
                    break;

                default:
                    AsLog.Error($"I haven't written a translation document for {language} yet φ(*￣0￣)");
                    break;
            }
        }

        /// <summary>
        /// 从文件获取翻译
        /// </summary>
        /// <param name="fileName">文件名</param>
        private static void FormatePoToDic(string fileName)
        {
            var content = AsFileManager.Local.ReadLines($@"AsTranslate\{fileName}.po").ToArray();

            if (content is null || content.Count() < 5)
            {
                AsLog.Error($"translate file {fileName}.po lost ＞﹏＜");

                return;
            }

            _translateDictionary.Clear();

            for (int i = 0; i < content.Length; i++)
            {
                //获取翻译开头
                if (content[i].StartsWith("msgid "))
                {
                    //如果是单行的翻译
                    if (content[i + 1].StartsWith("msgstr "))
                    {
                        var key = content[i].AsTryRemove("msgid ").AsTryRemove("\"");
                        var value = content[++i].AsTryRemove("msgstr ").AsTryRemove("\"");
                        if (key != "")
                        {
                            //AsLog.Debug($"get key {key}, value {value}");
                            _translateDictionary.Add(key, value);
                        }
                    }
                    //如果是多行翻译
                    else
                    {
                        //获取第一行的源文本
                        var key = new StringBuilder(content[i].AsTryRemove("msgid ").AsTryRemove("\""));
                        //阅读接下去的行数
                        for (int j = i + 1; j < content.Length; j++)
                        {
                            //添加接下去的每一行
                            if (!content[j].StartsWith("msgstr "))
                                key.Append(content[j].AsTryRemove("\""));
                            //如果到了翻译文本开头
                            else
                            {
                                //获取第一行的翻译文本
                                var value = new StringBuilder(content[j].AsTryRemove("msgstr ").AsTryRemove("\""));
                                //添加接下去的每一行
                                for (int k = j + 1; k < content.Length; k++)
                                {
                                    if (!content[k].StartsWith("#: "))
                                        value.Append(content[k].AsTryRemove("\""));
                                    else
                                    {
                                        //AsLog.Debug($"get pair Key{key} : value{value}");
                                        _translateDictionary.Add(key.ToString(), value.ToString());
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 允许字符串隐式转换为封装项
        /// </summary>
        /// <param name="value">字符</param>
        public static implicit operator AsString(string value) { return new AsString(value); }

        /// <summary>
        /// 允许封装项隐式转换为字符串, 此转换将会尝试进行翻译操作
        /// </summary>
        /// <param name="value">封装项</param>
        public static implicit operator string(AsString value) { return value.Translate(); }

        /// <summary>
        /// 此操作将会尝试进行翻译
        /// </summary>
        /// <returns>翻译结果</returns>
        public override string ToString()
        {
            return Translate();
        }
    }
}
