using AsTool.Assert;
using AsTool.Load;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AsTool.Unity.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class AsCommonMono
    {
        /// <summary>
        /// CommonMono的向外提供的可插入的生命周期
        /// </summary>
        public enum LifeCycle
        {
            /// <summary>
            /// 物理更新函数，循环执行，0.02 秒执行一次（不受 FPS 帧率影响，时间可更改），所有和物理相关的更新都应在此函数处理
            /// </summary>
            FixedUpdate,
            /// <summary>
            /// 更新函数，每帧执行一次，受 FPS 帧率影响
            /// </summary>
            Update,
            /// <summary>
            /// 稍后更新函数，在所有 Update 执行完后调用，帧间隔时间和 Update 一样
            /// </summary>
            LateUpdate,
            /// <summary>
            /// 在渲染和处理 GUI 事件时被调用，每帧都执行
            /// </summary>
            OnGUI
        }

        /// <summary>
        /// 内部用于执行 Mono behavior 行为的脚本
        /// </summary>
        private sealed class AsCommonMonoComponent : MonoBehaviour
        {
            private static readonly System.Random random = new System.Random();

            /// <summary>
            /// 保存了一个Mono行为
            /// </summary>
            private struct MonoAction
            {
                public MonoAction(string name, Action action)
                {
                    Name = name;
                    Action = action;
                }

                public string Name { get; private set; }

                public Action Action { get; private set; }
            }

            /// <summary>
            /// 私有的存放脚本的GameObject
            /// </summary>
            private static GameObject MonoObject { get; set; }

            /// <summary>
            /// 私有的存放脚本的地方
            /// </summary>
            private static AsCommonMonoComponent instance;

            /// <summary>
            /// 线程锁，用于保护多线程获取Instance的安全
            /// </summary>
            private static readonly object _Instancelock = new object();

            /// <summary>
            /// 单例模式的实例
            /// </summary>
            public static AsCommonMonoComponent Instance
            {
                get
                {
                    //确保多线程安全
                    if (MonoObject == null)
                        lock (_Instancelock)
                            if (MonoObject == null)
                            {
                                //进行自身的构造
                                MonoObject = AsPersistentGameObject.GetObject("MonoSingleCase");
                                //添加脚本
                                instance = MonoObject.AddComponent<AsCommonMonoComponent>();
                                //标记启用Mono
                                //AsLog.Debug("CommonMono report: Mono started");
                            }
                    return instance;
                }
            }

            /// <summary>
            /// 一个用于管理插入和删除的线程锁，防止多线程操作字典
            /// </summary>
            private readonly object _lock = new object();

            /// <summary>
            /// 一个用于管理委托的字典
            /// </summary>
            private Dictionary<AsCommonMono.LifeCycle, List<MonoAction>> MonoActs { get; set; } = new Dictionary<AsCommonMono.LifeCycle, List<MonoAction>>();

            /// <summary>
            /// 一个用于管理单次委托的字典
            /// </summary>
            private Dictionary<AsCommonMono.LifeCycle, Queue<MonoAction>> SingleTimeMonoActs { get; set; } = new Dictionary<AsCommonMono.LifeCycle, Queue<MonoAction>>();

            /// <summary>
            /// 尝试向目标生命周期添加行为，线程安全
            /// </summary>
            /// <param name="name">行为的名字（不可重复）</param>
            /// <param name="act">目标行为</param>
            /// <param name="lifeCycle">目标生命周期</param>
            /// <param name="isSingleAct">是否仅执行一次</param>
            /// <returns>添加成功返回真，否则返回假</returns>
            /// <exception cref="ArgumentNullException">参数不可为null</exception>
            public bool SetLifeCycleAction(string name, Action act, AsCommonMono.LifeCycle lifeCycle, bool isSingleAct)
            {
                AsAssert.NotNull(name, "SetLifeCycleAction get a null name");
                AsAssert.NotNull(act, "SetLifeCycleAction get a null act");

                if (!isSingleAct)
                    lock (_lock)
                    {
                        if (!MonoActs.ContainsKey(lifeCycle))
                            MonoActs.Add(lifeCycle, new List<MonoAction>());

                        //如果目标字符没有被占用
                        if (!MonoActs[lifeCycle].Any((monoAct) => monoAct.Name == name))
                        {
                            MonoActs[lifeCycle].Add(new MonoAction(name, act));
                            return true;
                        }

                        //如果目标字符被占用了
                        return false;
                    }
                else
                    lock (_lock)
                    {
                        if (!SingleTimeMonoActs.ContainsKey(lifeCycle))
                            SingleTimeMonoActs.Add(lifeCycle, new Queue<MonoAction>());

                        //如果目标字符没有被占用
                        if (!SingleTimeMonoActs[lifeCycle].Any((monoAct) => monoAct.Name == name))
                        {
                            SingleTimeMonoActs[lifeCycle].Enqueue(new MonoAction(name, act));
                            return true;
                        }

                        //如果目标字符被占用了
                        return false;
                    }
            }

            /// <summary>
            /// 尝试向目标生命周期移除一个非单次的行为，线程安全
            /// </summary>
            /// <param name="name">行为的名字</param>
            /// <param name="lifeCycle">目标生命周期</param>
            /// <returns>移除成功返回真，否则返回假</returns>
            /// <exception cref="ArgumentNullException">参数不可为null</exception>
            public bool RemoveLifeCycleAction(string name, AsCommonMono.LifeCycle lifeCycle)
            {
                AsAssert.NotNull(name, "RemoveLifeCycleAction get a null name");

                lock (_lock)
                {
                    bool IsMatch(MonoAction monoAct) => monoAct.Name == name;

                    if (!MonoActs.ContainsKey(lifeCycle))
                        return false;

                    if (!MonoActs[lifeCycle].Any(IsMatch))
                        return false;

                    MonoActs[lifeCycle].RemoveAt(MonoActs[lifeCycle].FindIndex(IsMatch));

                    if (!MonoActs[lifeCycle].Any())
                        MonoActs.Remove(lifeCycle);

                    return true;
                }
            }

            /// <summary>
            /// 根据周期的不同，执行对应的周期函数
            /// </summary>
            /// <param name="lifeCycle">目标周期</param>
            void DoCycleActions(LifeCycle lifeCycle)
            {
                void DoAct(Action action, string name)
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        string GetText(Exception outterEx)
                        {
                            if (outterEx.InnerException == null)
                                return outterEx.Message;

                            return outterEx.Message + " -- " + GetText(outterEx.InnerException);
                        }

                        AsLog.Fatal($"AsInnerCommonMono {lifeCycle} get a Exception while do {name}: {GetText(ex)}");
                    }
                }

                lock (_lock)
                {
                    if (MonoActs.TryGetValue(lifeCycle, out List<MonoAction> targetLis))
                        targetLis.ForEach((monoact) => DoAct(monoact.Action, monoact.Name));

                    if (SingleTimeMonoActs.TryGetValue(lifeCycle, out Queue<MonoAction> targetQue))
                    {
                        while (targetQue.Any())
                        {
                            var current = targetQue.Dequeue();
                            DoAct(current.Action, current.Name);
                        }
                    }
                }
            }

            public void FixedUpdate()
            {
                DoCycleActions(AsCommonMono.LifeCycle.FixedUpdate);
            }

            public void Update()
            {
                DoCycleActions(AsCommonMono.LifeCycle.Update);
            }

            public void LateUpdate()
            {
                DoCycleActions(AsCommonMono.LifeCycle.LateUpdate);
            }

            public void OnGUI()
            {
                DoCycleActions(AsCommonMono.LifeCycle.OnGUI);
            }

            private Dictionary<string, Coroutine> CoroutineDic { get; set; } = new Dictionary<string, Coroutine>();

            public readonly string Title = "-- AsCommonMonoComponent atuo name -- temp name number [" + random.NextDouble() + "] \n";

            /// <summary>
            /// 异步地添加一个协程行为，线程安全
            /// </summary>
            /// <param name="name">行为的名字</param>
            /// <param name="act">具体执行的内容</param>
            /// <returns>是否添加成功</returns>
            public async Task<bool> SetCoroutineAsync(string name, IEnumerator act)
            {
                var result = Task.Run(() => InnerSetCoroutineAsync(name, act));

                return await result;
            }

            /// <summary>
            /// 添加一个协程行为，线程不安全
            /// </summary>
            /// <param name="name">行为的名字</param>
            /// <param name="act">具体执行的内容</param>
            /// <returns>是否添加成功</returns>
            public bool SetCoroutine(string name, IEnumerator act)
            {
                lock (_lock)
                {
                    if (CoroutineDic.ContainsKey(name))
                        return false;

                    CoroutineDic.Add(name, StartCoroutine(act));

                    return true;
                }
            }

            /// <summary>
            /// 异步地添加一个协程行为，线程安全
            /// </summary>
            /// <param name="name">行为的名字</param>
            /// <param name="act">具体执行的内容</param>
            /// <returns>是否添加成功</returns>
            private bool InnerSetCoroutineAsync(string name, IEnumerator act)
            {
                ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                bool succeed = false;
                void Inneract()
                {
                    if (CoroutineDic.ContainsKey(name))
                    {
                        succeed = false;
                        manualResetEvent.Set();
                        return;
                    }

                    CoroutineDic.Add(name, StartCoroutine(act));

                    succeed = true;
                    manualResetEvent.Set();
                }

                while (!SetLifeCycleAction(
                    Title + $"<<InnerSetCoroutineAsync_{name}_{random.Next(int.MinValue, int.MaxValue)}>>",
                    Inneract,
                    AsCommonMono.LifeCycle.Update,
                    true))
                { };

                manualResetEvent.WaitOne();
                return succeed;
            }

            /// <summary>
            /// 移除一个协程行为，线程安全
            /// </summary>
            /// <param name="name">行为的名字</param>
            /// <returns>是否移除成功</returns>
            public async Task<bool> RemoveCoroutineAsync(string name)
            {
                var result = Task.Run(() => InnerRemoveCoroutineAsync(name));

                return await result;
            }

            /// <summary>
            /// 移除一个协程行为，线程安全
            /// </summary>
            /// <param name="name">行为的名字</param>
            /// <returns>是否移除成功</returns>
            private bool InnerRemoveCoroutineAsync(string name)
            {
                ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                bool succeed = false;
                void Inneract()
                {
                    if (!CoroutineDic.ContainsKey(name))
                    {
                        succeed = false;
                        manualResetEvent.Set();
                        return;
                    }


                    if (CoroutineDic[name] == null)
                    {
                        succeed = false;
                        manualResetEvent.Set();
                        return;
                    }

                    StopCoroutine(CoroutineDic[name]);

                    CoroutineDic.Remove(name);

                    succeed = true;
                    manualResetEvent.Set();
                }

                while (!SetLifeCycleAction(
                    Title + $"<<InnerRemoveCoroutineAsync_{name}_{random.Next(int.MinValue, int.MaxValue)}>>",
                    Inneract,
                    AsCommonMono.LifeCycle.Update,
                    true))
                { };

                manualResetEvent.WaitOne();
                return succeed;
            }

            /// <summary>
            /// 移除一个协程行为，线程不安全
            /// </summary>
            /// <param name="name">行为的名字</param>
            /// <returns>是否移除成功</returns>
            public bool RemoveCoroutine(string name)
            {
                lock (_lock)
                {
                    if (!CoroutineDic.ContainsKey(name))
                        return false;

                    if (CoroutineDic[name] == null)
                        return false;

                    StopCoroutine(CoroutineDic[name]);

                    CoroutineDic.Remove(name);

                    return true;
                }
            }

            /// <summary>
            /// 销毁实例。会把相应的类销毁掉
            /// </summary>
            public void DestroyInstance()
            {
                lock (_Instancelock)
                {
                    MonoObject = null;
                    MonoActs.Clear();
                    SingleTimeMonoActs.Clear();
                    CoroutineDic.Clear();
                    instance = null;
                    AsPersistentGameObject.RemoveObject("MonoSingleCase");
                }
            }
        }

        /// <summary>
        /// 在多线程调用之前必须在主线程中启用mono
        /// </summary>
        [AsLoad]
        internal static void StartMono()
        {
            try
            {
                AsCommonMonoComponent.Instance.SetLifeCycleAction(AsCommonMonoComponent.Instance.Title + "<<AsCommonMono_test_when_start>>", () => { }, LifeCycle.Update, true);
            }
            catch (Exception ex)
            {
                string GetText(Exception outterEx)
                {
                    if (outterEx.InnerException == null)
                        return outterEx.Message;

                    return outterEx.Message + " -- " + GetText(outterEx.InnerException);
                }

                AsLog.Fatal($"AsCommonMono get a Exception while try start AsCommonMono: {GetText(ex)}");
            }
        }

        /// <summary>
        /// 尝试向目标生命周期添加行为，线程安全
        /// </summary>
        /// <param name="name">行为的名字（不可重复），不能以 双左括折号 开头</param>
        /// <param name="act">目标行为</param>
        /// <param name="lifeCycle">目标生命周期</param>
        /// <param name="isSingleAct">目标行为是否仅执行一次就销毁</param>
        /// <returns>添加成功返回真，否则返回假</returns>
        public static bool SetLifeCycleAction(string name, Action act, LifeCycle lifeCycle, bool isSingleAct = false)
        {
            AsAssert.NotNull(name, "SetLifeCycleAction get a null name");
            AsAssert.NotNull(act, "SetLifeCycleAction get a null act");

            if (name.StartsWith(AsCommonMonoComponent.Instance.Title) == true)
                throw new ArgumentException($"act name can not start with \"{AsCommonMonoComponent.Instance.Title}\"");

            return AsCommonMonoComponent.Instance.SetLifeCycleAction(name, act, lifeCycle, isSingleAct);
        }

        /// <summary>
        /// 尝试向目标生命周期移除一个非单次的行为，线程安全
        /// </summary>
        /// <param name="name">行为的名字，不能以 双左括折号 开头</param>
        /// <param name="lifeCycle">目标生命周期</param>
        /// <returns>移除成功返回真，否则返回假</returns>
        public static bool RemoveLifeCycleAction(string name, LifeCycle lifeCycle)
        {
            AsAssert.NotNull(name, "RemoveLifeCycleAction get a null name");

            if (name.StartsWith(AsCommonMonoComponent.Instance.Title) == true)
                throw new ArgumentException($"act name can not start with \"{AsCommonMonoComponent.Instance.Title}\"");
            return AsCommonMonoComponent.Instance.RemoveLifeCycleAction(name, lifeCycle);
        }

        /// <summary>
        /// 添加一个协程行为
        /// </summary>
        /// <param name="name">行为的名字，不能以 双左括折号 开头</param>
        /// <param name="act">具体执行的内容</param>
        /// <returns>是否添加成功</returns>
        public static bool SetCoroutine(string name, IEnumerator act)
        {
            AsAssert.NotNull(name, "SetCoroutine get a null name");

            if (name.StartsWith(AsCommonMonoComponent.Instance.Title) == true)
                throw new ArgumentException($"act name can not start with \"{AsCommonMonoComponent.Instance.Title}\"");
            return AsCommonMonoComponent.Instance.SetCoroutine(name, act);
        }

        /// <summary>
        /// 异步地添加一个协程行为，线程安全但是更慢
        /// </summary>
        /// <param name="name">行为的名字，不能以 双左括折号 开头</param>
        /// <param name="act">具体执行的内容</param>
        /// <returns>是否添加成功</returns>
        public static async Task<bool> SetCoroutineAsync(string name, IEnumerator act)
        {
            AsAssert.NotNull(name, "SetCoroutineAsync get a null name");

            if (name.StartsWith(AsCommonMonoComponent.Instance.Title) == true)
                throw new ArgumentException($"act name can not start with \"{AsCommonMonoComponent.Instance.Title}\"");
            return await AsCommonMonoComponent.Instance.SetCoroutineAsync(name, act);
        }

        /// <summary>
        /// 移除一个协程行为
        /// </summary>
        /// <param name="name">行为的名字，不能以 双左括折号 开头</param>
        /// <returns>是否移除成功</returns>
        public static bool ReMoveCoroutine(string name)
        {
            AsAssert.NotNull(name, "ReMoveCoroutine get a null name");

            if (name.StartsWith(AsCommonMonoComponent.Instance.Title) == true)
                throw new ArgumentException($"act name can not start with \"{AsCommonMonoComponent.Instance.Title}\"");
            return AsCommonMonoComponent.Instance.RemoveCoroutine(name);
        }

        /// <summary>
        /// 异步移除一个协程行为，线程安全但是更慢
        /// </summary>
        /// <param name="name">行为的名字，不能以 双左括折号 开头</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<bool> RemoveCoroutineAsync(string name)
        {
            AsAssert.NotNull(name, "RemoveCoroutineAsync get a null name");

            if (name.StartsWith(AsCommonMonoComponent.Instance.Title) == true)
                throw new ArgumentException($"act name can not start with \"{AsCommonMonoComponent.Instance.Title}\"");
            return await AsCommonMonoComponent.Instance.RemoveCoroutineAsync(name);
        }

        /// <summary>
        /// 销毁实例。会把相应的脚本销毁掉
        /// </summary>
        public static void DestroyInstance()
        {
            AsCommonMonoComponent.Instance.DestroyInstance();
        }

    }
}
