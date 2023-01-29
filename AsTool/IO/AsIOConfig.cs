using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AsTool.IO
{
    /// <summary>
    /// 数据输入输出的各类信息
    /// </summary>
    public static class AsIOConfig
    {
        /// <summary>
        /// 所有的操作文件行为在操作时都需要持有此锁以保证线程安全
        /// </summary>
        internal static object FileOptionLock { get; private set; } = new object();

        /// <summary>
        /// 本地文件夹的根目录
        /// </summary>
        private static string modLoacalPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Local");

        /// <summary>
        /// 本地文件夹的根目录
        /// </summary>
        public static string LocalPath { get { return modLoacalPath; } }

        /// <summary>
        /// 目标文件夹的根目录
        /// </summary>
        private static string targetPath = Directory.GetCurrentDirectory();

        /// <summary>
        /// 目标文件夹的根目录
        /// </summary>
        public static string TargetPath { get { return targetPath; } }

        /// <summary>
        /// 混合所有相对路径，获得程序集 (mod文件) 所在的绝对根路径，不会进行null检查
        /// </summary>
        /// <param name="paths">输入的相对路径</param>
        /// <returns>绝对路径</returns>
        public static string GetLoaclFullPath(params string[] paths)
        {
            var path = string.Empty;

            foreach (string s in paths)
            {
                if (!string.IsNullOrEmpty(s))
                    path = Path.Combine(path, s);
            }

            if (!string.IsNullOrEmpty(path))
                return Path.Combine(LocalPath, path);

            return LocalPath;
        }

        /// <summary>
        /// 混合所有相对路径，获得主程序集所在绝对根路径，不会进行null检查
        /// </summary>
        /// <param name="paths">输入的相对路径</param>
        /// <returns>绝对路径</returns>
        public static string GetTargetFullPath(params string[] paths)
        {
            var path = string.Empty;

            foreach (string s in paths)
            {
                if (!string.IsNullOrEmpty(s))
                    path = Path.Combine(path, s);
            }

            return Path.Combine(TargetPath, path);
        }

        /// <summary>
        /// 设置本地文件夹和主程序集文件夹
        /// </summary>
        /// <param name="localpath">程序集 (mod文件)文件夹的根目录</param>
        /// <param name="targetpath">主程序集文件夹的根目录</param>
        public static void SetPath(string localpath, string targetpath)
        {
            modLoacalPath = localpath;
            targetPath = targetpath;
        }
    }
}
