namespace Destroy
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CreatGameObject : Attribute
    {
        public string Name;

        public uint CreatOrder;

        /// <summary>
        /// creatOrder为0时脚本执行优先级最高, 然后向着数轴正方向递减。
        /// </summary>
        public CreatGameObject(uint creatOrder = uint.MaxValue, string name = "GameObject")
        {
            Name = name;
            CreatOrder = creatOrder;
        }
    }
}