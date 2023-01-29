using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.IO
{
    internal static class AsIOActions
    {
        /// <summary>
        /// 尝试触碰一个地址，如果该地址不存在就创建一个
        /// </summary>
        /// <param name="path">地址（绝对）</param>
        /// <param name="FileName">如果是一个文件的地址为真，文件夹地址为假</param>
        /// <returns>该地址是否已经存在</returns>
        public static bool Touch(string path, bool FileName = true)
        {
            if (!FileName)
            {
                if (Directory.Exists(path))
                {
                    return true;
                }

                Directory.CreateDirectory(path);
                return false;
            }

            bool e = true;

            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                e = false;

                Directory.CreateDirectory(dir);
            }

            if (!File.Exists(path))
            {
                e = false;

                File.Create(path).Dispose();
            }

            return e;

        }

        /// <summary>
        /// 输入一个文件路径，如果存在目标文件或者目录就删除
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="FileName">如果是一个文件的地址为真，文件夹地址为假</param>
        /// <returns>是否找到并删除</returns>
        public static bool FileDestory(string path, bool FileName)
        {
            if (FileName && File.Exists(path))
            {
                File.Delete(path);

                return true;
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 尝试向一个文件写入所有字符串
        /// </summary>
        /// <param name="content">字符串内容</param>
        /// <param name="path">写入路径</param>
        public static void WriteFile(string content, string path)
        {
            Touch(path);

            File.WriteAllText(path, content);
        }

        /// <summary>
        /// 批量地写入文件
        /// </summary>
        /// <param name="content">文件内容 -> 文件名(带后缀名) - 内容</param>
        /// <param name="path">写入文件的文件夹</param>
        public static void WriteFiles(Dictionary<string, string> content, string path)
        {
            Touch(path, false);

            foreach (var kvp in content)
            {
                WriteFile(kvp.Value, Path.Combine(path, kvp.Key));
            }
        }

        /// <summary>
        /// 尝试读入一个文件的所有字符串
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>字符串内容</returns>
        public static string ReadFile(string path)
        {
            Touch(path);

            return File.ReadAllText(path);
        }

        /// <summary>
        /// 以行为单位读入文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>字符串内容</returns>
        public static IEnumerable<string> ReadLines(string path)
        {
            Touch(path);

            return File.ReadLines(path);
        }

        /// <summary>
        /// 批量地读取文件
        /// </summary>
        /// <param name="path">写入文件的文件夹</param>
        /// <returns>文件名 - 内容</returns>
        public static Dictionary<string, string> ReadFiles(string path)
        {
            Touch(path, false);

            var result = new Dictionary<string, string>();

            foreach (var item in Directory.GetFiles(path))
            {
                result.Add(item, ReadFile(item));
            }

            return result;
        }

        /// <summary>
        /// 尝试向一个文件写入所有二进制内容
        /// </summary>
        /// <param name="content">二进制内容</param>
        /// <param name="path">写入路径</param>
        public static void WriteByteFile(byte[] content, string path)
        {
            Touch(path);

            File.WriteAllBytes(path, content);
        }

        /// <summary>
        /// 尝试读入一个文件的所有二进制内容
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>二进制内容</returns>
        public static byte[] ReadByteFile(string path)
        {
            Touch(path);

            return File.ReadAllBytes(path);
        }
    }
}
