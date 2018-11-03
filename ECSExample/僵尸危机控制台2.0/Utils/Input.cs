using System;
using System.Runtime.InteropServices;

namespace ZombieInfection
{
    /// <summary>
    /// 鼠标相关
    /// </summary>
    public enum MouseButton
    {
        LeftMouseButton = 0x01,
        RightMouseButton = 0x02,
        MiddleMouseButton = 0x04,
    }

    /// <summary>
    /// 键盘按键
    /// </summary>
    public enum KeyCode
    {
        None = 0,           //空
        BackSpace = 0x08,   //退格
        Tab = 0x09,         //TAB
        ESC = 0x1B,         //Esc
        Space = 0x20,       //空格
        //有两个按键的
        Enter = 0x0D,       //回车
        Shift = 0x10,       //Shift
        Ctrl = 0x11,        //Control
        Alt = 0x12,         //ALT
        //A-Z
        A = 0x41,
        B = 0x42,
        C = 0x43,
        D = 0x44,
        E = 0x45,
        F = 70,
        G = 0x47,
        H = 0x48,
        I = 0x49,
        J = 0x4a,
        K = 0x4b,
        L = 0x4c,
        M = 0x4d,
        N = 0x4e,
        O = 0x4f,
        P = 80,
        Q = 0x51,
        R = 0x52,
        S = 0x53,
        T = 0x54,
        U = 0x55,
        V = 0x56,
        W = 0x57,
        X = 0x58,
        Y = 0x59,
        Z = 90,
        //数字0-9
        _0 = 0x30,
        _1 = 0x31,
        _2 = 0x32,
        _3 = 0x33,
        _4 = 0x34,
        _5 = 0x35,
        _6 = 0x36,
        _7 = 0x37,
        _8 = 0x38,
        _9 = 0x39,
    }

    /// <summary>
    /// 输入类 <see langword="static"/>
    /// </summary>
    public static class Input
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int key);

        /// <summary>
        /// 上一次按下的键
        /// </summary>
        private static KeyCode LastInputKey { get; set; }

        /// <summary>
        /// 上一次松开的键
        /// </summary>
        private static KeyCode LastDropKey { get; set; }

        /// <summary>
        /// 获取当前按下的键
        /// </summary>
        private static KeyCode GetCurrentKey()
        {
            KeyCode inputKey = KeyCode.None;
            foreach (int key in Enum.GetValues(typeof(KeyCode)))
            {
                if (GetKey((KeyCode)key))
                {
                    inputKey = (KeyCode)key;
                    break;
                }
            }
            return inputKey;
        }

        #region 公开接口

        /// <summary>
        /// 获取持续的按键输入
        /// </summary>
        public static bool GetKey(KeyCode keyCode) => GetAsyncKeyState((int)keyCode) != 0;

        /// <summary>
        /// 获取按下指定按键
        /// </summary>
        public static bool GetKeyDown(KeyCode keyCode)
        {
            //如果没有按当前键就获取当前按的键
            if (!GetKey(keyCode))
                LastInputKey = GetCurrentKey();
            //按下了指定键并且当前键不等于上次一键
            if (GetKey(keyCode) && keyCode != LastInputKey)
            {
                //更新上一次键
                LastInputKey = keyCode;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取松开指定按键
        /// </summary>
        public static bool GetKeyUp(KeyCode keyCode)
        {
            KeyCode inputKeyCode = GetCurrentKey();
            //按下了指定键
            if (inputKeyCode == keyCode)
            {
                LastDropKey = keyCode;
                return false;
            }
            //松手离开指定键
            if (LastDropKey == keyCode && inputKeyCode != LastDropKey)
            {
                LastDropKey = inputKeyCode;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取指定方向上的输入
        /// </summary>
        public static int GetDirectInput(KeyCode negative, KeyCode positive)
        {
            int result = 0;
            if (GetKey(negative))
                result -= 1;
            if (GetKey(positive))
                result += 1;

            return result;
        }

        #endregion
    }
}