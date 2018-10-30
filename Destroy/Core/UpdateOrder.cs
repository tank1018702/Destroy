using System;

namespace Destroy
{
    /// <summary>
    /// Order为0时执行优先级最高, 然后向着数轴正方向递减。
    /// 指定了UpdateOrder特性的会比没有指定的执行优先级更高。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UpdateOrder : Attribute
    {
        public uint Order;

        public UpdateOrder(uint order) => Order = order;
    }
}