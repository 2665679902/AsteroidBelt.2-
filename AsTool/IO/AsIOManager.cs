using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.IO
{
    /// <summary>
    /// 管理文件处理方法的工具类
    /// </summary>
    public static class AsFileManager
    {
        private static object Lock { get => AsIOConfig.FileOptionLock; }

        /// <summary>
        /// 管理本地文件
        /// </summary>
        public static class Local
        {
            private static string GetFullPath(params string[] paths) => AsIOConfig.GetLoaclFullPath(paths);

            /// <summary>
            /// 尝试触碰一个本地地址，如果该地址不存在就创建一个
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            /// <param name="fileName">是否输入的是文件名</param>
            /// <returns>该地址是否存在</returns>
            public static bool Touch(string path, bool absolutePath = false, bool fileName = true)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                return AsIOActions.Touch(path, fileName);
            }

            /// <summary>
            /// 尝试向一个本地文件夹的文件写入所有字符串
            /// </summary>
            /// <param name="content">字符串内容</param>
            /// <param name="path">写入路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            public static void WriteFile(string content, string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    AsIOActions.WriteFile(content, path);
            }

            /// <summary>
            /// 尝试向一个本地文件夹批量地写入文件
            /// </summary>
            /// <param name="content">文件内容 -> 文件名(带后缀名) - 内容</param>
            /// <param name="path">写入文件的文件夹</param>
            /// <param name="absolutePath">是否是绝对路径</param>
            public static void WriteFiles(Dictionary<string, string> content, string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    AsIOActions.WriteFiles(content, path);

            }

            /// <summary>
            /// 尝试读入一个本地文件夹的文件的所有字符串
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            /// <returns>字符串内容</returns>
            public static string ReadFile(string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.ReadFile(path);
            }

            /// <summary>
            /// 尝试读入一个本地文件夹的以行为单位的所有字符串
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            /// <returns>字符串内容</returns>
            public static IEnumerable<string> ReadLines(string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.ReadLines(path);
            }

            /// <summary>
            /// 尝试从一个本地文件夹批量地读取文件
            /// </summary>
            /// <param name="path">写入文件的文件夹</param>
            /// <param name="absolutePath">是否是绝对路径</param>
            /// <returns>文件名 - 内容</returns>
            public static Dictionary<string, string> ReadFiles(string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.ReadFiles(path);
            }

            /// <summary>
            /// 尝试向一个本地文件文件写入所有二进制内容
            /// </summary>
            /// <param name="content">二进制内容</param>
            /// <param name="path">写入路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            public static void WriteByteFile(byte[] content, string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    AsIOActions.WriteByteFile(content, path);
            }

            /// <summary>
            /// 尝试读入一个本地文件文件的所有二进制内容
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            /// <returns>二进制内容</returns>
            public static byte[] ReadByteFile(string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.ReadByteFile(path);
            }

            /// <summary>
            /// 输入一个文件路径，如果存在目标文件或者目录就删除
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            /// <param name="fileName">是否是文件名</param>
            /// <returns>是否找到并删除</returns>
            public static bool FileDestory(string path, bool absolutePath = false, bool fileName = true)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.FileDestory(path, fileName);
            }


            private static readonly Dictionary<string, List<string>> _fileLoaded = new Dictionary<string, List<string>>();

            /// <summary>
            /// 将一个文件从本地文件夹拷贝到目标文件夹
            /// </summary>
            /// <param name="localpath">本地路径</param>
            /// <param name="targetpath">目标路径</param>
            /// <param name="absolutePath">是否是绝对路径</param>
            public static void Copy(string localpath, string targetpath, bool absolutePath = false)
            {
                if (!absolutePath)
                {
                    localpath = Path.Combine(AsIOConfig.LocalPath, localpath);
                    targetpath = Path.Combine(AsIOConfig.TargetPath, targetpath);
                }

                lock (Lock)
                {
                    var content = AsIOActions.ReadByteFile(localpath);

                    AsIOActions.WriteByteFile(content, targetpath);
                }
            }

            /// <summary>
            /// 加载本地文件夹的所有文件到目标文件夹
            /// </summary>
            /// <param name="localpath">本地文件夹</param>
            /// <param name="targetpath">目标文件夹</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            public static void LoadFile(string localpath, string targetpath, bool absolutePath = false)
            {
                if (!absolutePath)
                {
                    localpath = Path.Combine(AsIOConfig.LocalPath, localpath);
                    targetpath = Path.Combine(AsIOConfig.TargetPath, targetpath);
                }

                AsIOActions.Touch(localpath, false);

                lock (Lock)
                {
                    if (!_fileLoaded.ContainsKey(localpath))
                        _fileLoaded[localpath] = new List<string>();

                    foreach (var file in Directory.GetFiles(localpath))
                    {
                        var TargetFile = Path.Combine(targetpath, Path.GetFileName(file));
                        _fileLoaded[localpath].Add(TargetFile);
                        AsIOActions.WriteByteFile(AsIOActions.ReadByteFile(file), TargetFile);
                    }
                }
            }

            /// <summary>
            /// 将本次加载的所有文件都删除
            /// </summary>
            /// <param name="localpath">要删除的加载的文件夹</param>
            /// <param name="absolutePath">是否使用了绝对路径</param>
            public static void UnLoadFile(string localpath, bool absolutePath = false)
            {
                if (!absolutePath)
                    localpath = GetFullPath(localpath);

                lock (Lock)
                    if (_fileLoaded.ContainsKey(localpath))
                    {
                        foreach (var file in _fileLoaded[localpath])
                        {
                            AsIOActions.FileDestory(file, true);
                        }

                        _fileLoaded.Remove(localpath);
                    }
            }
        }

        /// <summary>
        /// 管理目标文件
        /// </summary>
        public static class Target
        {
            private static string GetFullPath(params string[] paths) => AsIOConfig.GetTargetFullPath(paths);

            /// <summary>
            /// 尝试触碰一个地址，如果该地址不存在就创建一个
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            /// <returns>该地址是否存在</returns>
            public static bool Touch(string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.Touch(path);
            }

            /// <summary>
            /// 尝试向一个目标文件夹的文件写入所有字符串
            /// </summary>
            /// <param name="content">字符串内容</param>
            /// <param name="path">写入路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            public static void WriteFile(string content, string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    AsIOActions.WriteFile(content, path);
            }

            /// <summary>
            /// 尝试向一个目标文件夹批量地写入文件
            /// </summary>
            /// <param name="content">文件内容 -> 文件名(带后缀名) - 内容</param>
            /// <param name="path">写入文件的文件夹</param>
            /// <param name="absolutePath">是否是绝对路径</param>
            public static void WriteFiles(Dictionary<string, string> content, string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    AsIOActions.WriteFiles(content, path);

            }

            /// <summary>
            /// 尝试读入一个目标文件夹的文件的所有字符串
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            /// <returns>字符串内容</returns>
            public static string ReadFile(string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.ReadFile(path);
            }

            /// <summary>
            /// 尝试读入一个目标文件夹的以行为单位的所有字符串
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            /// <returns>字符串内容</returns>
            public static IEnumerable<string> ReadLines(string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.ReadLines(path);
            }

            /// <summary>
            /// 尝试从一个目标文件夹批量地读取文件
            /// </summary>
            /// <param name="path">写入文件的文件夹</param>
            /// <param name="absolutePath">是否是绝对路径</param>
            /// <returns>文件名 - 内容</returns>
            public static Dictionary<string, string> ReadFiles(string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.ReadFiles(path);
            }

            /// <summary>
            /// 尝试向一个目标文件文件写入所有二进制内容
            /// </summary>
            /// <param name="content">二进制内容</param>
            /// <param name="path">写入路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            public static void WriteByteFile(byte[] content, string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    AsIOActions.WriteByteFile(content, path);
            }

            /// <summary>
            /// 尝试读入一个目标文件文件的所有二进制内容
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            /// <returns>二进制内容</returns>
            public static byte[] ReadByteFile(string path, bool absolutePath = false)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.ReadByteFile(path);
            }

            /// <summary>
            /// 输入一个文件路径，如果存在目标文件或者目录就删除
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="absolutePath">是否输入了绝对路径</param>
            /// <param name="fileName">是否是文件名</param>
            /// <returns>是否找到并删除</returns>
            public static bool FileDestory(string path, bool absolutePath = false, bool fileName = true)
            {
                if (!absolutePath)
                    path = GetFullPath(path);

                lock (Lock)
                    return AsIOActions.FileDestory(path, fileName);
            }
        }
    }
}
