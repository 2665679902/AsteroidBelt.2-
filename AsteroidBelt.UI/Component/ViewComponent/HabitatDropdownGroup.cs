using AsteroidBelt.Data;
using AsteroidBelt.Data.String.Messages;
using AsteroidBelt.Data.templates.Building.Rocket.Habitat;
using AsteroidBelt.UI.UIEvent;
using AsTool.Assert;
using AsTool.Event;
using AsTool.Extension;
using AsTool.Unity.Component.EventComponent;
using AsTool.Unity.Component.UIComponent;
using AsTool.Unity.Component.UIComponent.DefaultUIComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AsteroidBelt.UI.Component.ViewComponent
{
    /// <summary>
    /// 设置火箭仓的下拉框组件
    /// </summary>
    public class HabitatDropdownGroup: AsUIComponent
    {
        /// <summary>
        /// 当前生成的唯一实例
        /// </summary>
        public static HabitatDropdownGroup Instance { get; private set; }

        /// <summary>
        /// 作为模板使用的下拉框
        /// </summary>
        [Tooltip("作为模板使用的下拉框")]
        public AsDropdown Template;

        /// <summary>
        /// 脚本当前持有数据
        /// </summary>
        public HugeHabitatData HabitatData;

        /// <summary>
        /// 初始化时失活模板, 获取自动布局组件
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            AsAssert.NotNull(Template, "HabitateDropdown Template lost!");

            Template.gameObject.SetActive(false);

            //将其调整到默认格式
            layoutGroup = GetOrAddComponent<GridLayoutGroup>();
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            layoutGroup.constraintCount = 1;
            layoutGroup.cellSize = Template.GetComponent<RectTransform>().sizeDelta;
            layoutGroup.spacing = new Vector2(-(layoutGroup.cellSize.x * (Screen.width - 1920)/1920)/2  , -(layoutGroup.cellSize.y * (Screen.height - 1080) / 1080)/2);

            //刷新当前脚本的数据
            HabitatData = AsDataObject.RefreshFromFile<HugeHabitatData>() ?? new HugeHabitatData();

            if(HabitatData.HabitatDiscribe is null)
            {
                SetSize(20);
                SetDefault();
            }

            Instance = this;
        }

        /// <summary>
        /// 确保其他组件启动完成且可视后阅读信息
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            ReadData();
        }

        /// <summary>
        /// 在失活时删除相关组件，以保证帧率
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            foreach(var child in children)
            {
                Destroy(child.gameObject);
            }

            children.Clear();

            currentSize = 0;
        }

        /// <summary>
        /// 当前的可编辑尺寸
        /// </summary>
        private int currentSize = 0;

        /// <summary>
        /// 私有的格子自动布局组件
        /// </summary>
        private GridLayoutGroup layoutGroup;

        /// <summary>
        /// 保存所有的下拉框子项
        /// </summary>
        private List<AsDropdown> children = new List<AsDropdown>();

        /// <summary>
        /// 从加载的数据中获取信息
        /// </summary>
        private void ReadData()
        {
            int size = HabitatData.HabitatDiscribe.Count;

            //如果相等, 直接返回
            if (currentSize == size)
                return;

            AsAssert.IsTrue(size > 9, "size must bigger than 9");

            currentSize = size;
            layoutGroup.constraintCount = size;

            foreach(var i in HabitatData.HabitatDiscribeInList)
            {
                var drop = AddNewChild();

                drop.Value = i;
            }

            AsEvent.Trigger(SliderEvent.Get_HabitatDesignSizeChange.AsToString(), new AsComponentEventArg() { Sender = this, Data = (float)size });
            AsEvent.Trigger(InputFieldEvent.Get_HabitatSizeText.AsToString(), new AsComponentEventArg() { Sender = this, Data = size });
            AsEvent.Trigger(TextEvent.Get_HabitatConditionText.AsToString(), new AsComponentEventArg() { Sender = this, Data = AsHabitateString.Success.HabitatLoadSuccess.Translate() });
        }

        /// <summary>
        /// 创建一个新的子项
        /// </summary>
        /// <returns>创建的子项</returns>
        private AsDropdown AddNewChild()
        {
            var newChild = Instantiate(Template.gameObject);

            newChild.SetActive(true);

            newChild.transform.SetParent(transform);

            newChild.name += $"_{children.Count}";

            var drop = newChild.GetComponent<AsDropdown>();

            drop.ValueChanging += OnValueChange;

            children.Add(drop);

            return drop;
        }

        /// <summary>
        /// 移除一个子项
        /// </summary>
        /// <param name="Index">移除序号</param>
        private void RemoveChild(int Index)
        {
            AsAssert.IsTrue(Index < children.Count && Index >= 0, $"RemoveChild get a wrong index: {Index}");

            Destroy(children[Index].gameObject);

            children.RemoveAt(Index);
        }

        /// <summary>
        /// 设置当前的可编辑尺寸
        /// </summary>
        /// <param name="size">更改尺寸, 尺寸必须比10大</param>
        public void SetSize(int size)
        {
            //如果相等, 直接返回
            if(currentSize == size)
                return;

            AsAssert.IsTrue(size > 9, "size must bigger than 9");

            currentSize = size;
            layoutGroup.constraintCount = size;

            int count = size * size;

            while(children.Count > count)
            {
                RemoveChild(children.Count - 1);
            }

            while (children.Count < count)
            {
                _ = AddNewChild();
            }
        }

        /// <summary>
        /// 获取当前状态的对应数值
        /// </summary>
        /// <returns>数值</returns>
        public List<List<int>> GetValue()
        {
            var result = new List<List<int>>();
            var index = 0;

            for(int i = 0; i < currentSize; i++)
            {
                var item = new List<int>();

                for(int j = 0;j < currentSize; j++)
                {
                    item.Add(children[index].Value);

                    index++;
                }

                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// 保存当前的数据
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <returns>返回是否检查通过以保存成功</returns>
        public bool Save(out string message)
        {
            HabitatData.HabitatDiscribe = GetValue();

            if(!HabitatData.Check(out message))
            {
                return false;
            }

            HabitatData.Serialize();

            return true;
        }

        /// <summary>
        /// 将状态组设置为默认状态
        /// </summary>
        public void SetDefault()
        {
            //将所有子项依次入队
            var childQueue = new Queue<AsDropdown>();
            foreach(var child in children)
            {
                childQueue.Enqueue(child);
            }


            for (int i = 0; i < currentSize; i++)
            {
                //如果在第一行
                if(i == 0)
                {
                    //预计将每一个格子都设置成玻璃
                    for (int j = 0; j < currentSize; j++)
                    {
                        //将两边设置成墙
                        if(j == 0 || j == currentSize - 1)
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.RocketWallTile;
                        }
                        else
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.RocketEnvelopeWindowTile;
                        }
                    }
                }
                //如果在最后一行
                else if (i == currentSize - 1)
                {
                    //预计将每一个格子都设置成墙
                    for (int j = 0; j < currentSize; j++)
                    {
                        if(j == 1)
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.RocketInteriorGasInputPort;
                        }
                        else if(j == 2)
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.RocketInteriorGasOutputPort;
                        }
                        else if(j == currentSize - 3)
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.RocketInteriorLiquidOutputPort;
                        }
                        else if (j == currentSize - 2)
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.RocketInteriorLiquidInputPort;
                        }
                        else
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.RocketWallTile;
                        }
                    }

                }
                //如果在倒数 2 行
                else if(i == currentSize - 2)
                {
                    for(int j = 0; j < currentSize; j++)
                    {
                        //第一行是门
                        if(j == 0)
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.ClustercraftInteriorDoor;
                        }
                        //最后一行是墙
                        else if(j == currentSize - 1)
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.RocketWallTile;
                        }
                        //其他是真空
                        else
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.Vacuum;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < currentSize; j++)
                    {
                        //第一行和最后一行是墙
                        if (j == 0 || j == currentSize - 1)
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.RocketWallTile;
                        }
                        //其他是真空
                        else
                        {
                            childQueue.Dequeue().Value = HugeHabitatData.Vacuum;
                        }
                    }
                }
                
            }

            HabitatData.HabitatDiscribe = GetValue();

            AsAssert.IsTrue(HabitatData.Check(out var errorMessage), errorMessage);

            AsEvent.Trigger(TextEvent.Get_HabitatConditionText.AsToString(), new AsComponentEventArg() { Sender = this, Data = AsHabitateString.Success.HabitatOverwrittenSuccess.Translate() });
        }

        private void OnValueChange(int value)
        {

        }
    }
}
