using System;

namespace ZombieInfection
{
    /// <summary>
    /// 标识帮助类
    /// </summary>
    public static class FlagHelper
    {
        public static uint GetFlag(this string flag)
        {
            if (!Enum.TryParse(flag, out ComponentFlag result))
                throw new Exception("组件名字转换枚举失败");
            return (uint)result;
        }

        public static bool Is(this uint flag, uint flagA) => (flag ^ flagA) == 0;

        public static bool Has(this uint flag, uint flagA) => (flag & flagA) == flagA;

        public static uint Add(this uint flag, uint flagA) => flag | flagA;

        public static uint Remove(this uint flag, uint flagA) => flag ^ flagA;
    }
}