namespace Destroy
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CreatGameObject : Attribute
    {
        public string Name;

        public uint CreatOrder;

        public Type[] RequiredComponents;

        /// <summary>
        /// creatOrder为0时脚本执行优先级最高, 然后向着数轴正方向递减。
        /// </summary>
        public CreatGameObject(uint creatOrder = uint.MaxValue, string name = "GameObject", params Type[] requiredComponents)
        {
            Name = name;
            CreatOrder = creatOrder;
            RequiredComponents = requiredComponents;
        }
    }
}