using AsTool.Assert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Common
{
    /// <summary>
    /// 一个不会被销毁的GameObject类，一般作为所有类似类的parent
    /// </summary>
    public sealed class AsPersistentGameObject
    {
        private static readonly object _lock;

        /// <summary>
        /// 私有的存放实例的地方
        /// </summary>
        private static readonly GameObject gameObject;

        /// <summary>
        /// 存放子类，加快检索
        /// </summary>
        private static readonly Dictionary<string, List<GameObject>> Childs;

        /// <summary>
        /// 静态构造时就初始化存放点
        /// </summary>
        static AsPersistentGameObject()
        {
            gameObject = new GameObject("AsPersistentGameObject");

            Childs = new Dictionary<string, List<GameObject>>();

            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            _lock = new object();
        }

        /// <summary>
        /// 设置子对象
        /// </summary>
        /// <param go="Child">子对象</param>
        public static void SetChild(GameObject Child)
        {
            AsAssert.NotNull(Child, "PersistentGameObject: SetChild get null");

            lock (_lock)
            {
                Child.transform.SetParent(gameObject.transform);

                if (Childs.ContainsKey(Child.name))
                {
                    Childs[Child.name].Add(Child);
                }
                else
                {
                    Childs[Child.name] = new List<GameObject>() { Child };
                }
            }

        }

        /// <summary>
        /// 获取一个不会被销毁的对象
        /// </summary>
        /// <param go="name">对象名</param>
        /// <returns>对象</returns>
        public static GameObject GetObject(string name)
        {
            AsAssert.NotNull(name, "PersistentGameObject: GetObject get null");

            GameObject obj = new GameObject(name);
            SetChild(obj);
            return obj;
        }

        /// <summary>
        /// 尝试销毁相应的永久对象
        /// </summary>
        /// <param go="name">对象名字</param>
        /// <param go="immediate">是否立即清除</param>
        /// <returns>是否找到要清除的对象</returns>
        public static bool RemoveObject(string name, bool immediate = false)
        {
            AsAssert.NotNull(name, "PersistentGameObject: RemoveObject get null name");


            lock (_lock)
            {
                if (!Childs.ContainsKey(name))
                    return false;

                foreach (GameObject obj in Childs[name])
                {
                    if (obj != null)
                        if (immediate)
                            GameObject.DestroyImmediate(obj);
                        else
                            GameObject.Destroy(obj);
                }

                Childs.Remove(name);
                return true;
            }

        }

        /// <summary>
        /// 尝试销毁相应的永久对象
        /// </summary>
        /// <param go="go">对象</param>
        /// <param go="immediate">是否立即清除</param>
        /// <returns>是否找到要清除的对象</returns>
        public static bool RemoveObject(GameObject go, bool immediate = false)
        {
            AsAssert.NotNull(go, "PersistentGameObject: RemoveObject get null GameObject");

            lock (_lock)
            {
                if (!Childs.ContainsKey(go.name) || !Childs[go.name].Contains(go))
                    return false;

                Childs[go.name].Remove(go);

                if (immediate)
                    GameObject.DestroyImmediate(go);
                else
                    GameObject.Destroy(go);
                return true;
            }
        }
    }
}
