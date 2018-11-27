namespace Destroy
{
    using System;

    /// <summary>
    /// 键盘按键(104键)
    /// </summary>
    public enum KeyCode
    {
#if DoubleKey
        //Shift = 0x10,               // Shift
        //Ctrl = 0x11,                // Control
        //Alt = 0x12,                 // Alt
#endif

        None = 0,                   // 空
        Enter = 0x0d,               // Enter
        Delete = 0x2e,              // Delete

        #region Keyboard

        //第一行
        ESC = 0x1b,                 // esc
        F1 = 0x70,                  // F1
        F2 = 0x71,                  // F2
        F3 = 0x72,                  // F3
        F4 = 0x73,                  // F4
        F5 = 0x74,                  // F5
        F6 = 0x75,                  // F6
        F7 = 0x76,                  // F7
        F8 = 0x77,                  // F8
        F9 = 0x78,                  // F9
        F10 = 0x79,                 // F10
        F11 = 0x7A,                 // F11
        F12 = 0x7B,                 // F12
        PrintScreen = 0x2c,         // ptr sc(sys rq)
        ScrollLock = 0x91,          // scroll
        PauseBreak = 0x13,          // pause break
        //第二行
        _Num1 = 0xc0,               // ` ~
        Num1 = 0x31,                // 1 !
        Num2 = 0x32,                // 2 @
        Num3 = 0x33,                // 3 #
        Num4 = 0x34,                // 4 $
        Num5 = 0x35,                // 5 %
        Num6 = 0x36,                // 6 ^
        Num7 = 0x37,                // 7 &
        Num8 = 0x38,                // 8 *
        Num9 = 0x39,                // 9 (
        Num0 = 0x30,                // 0 )
        MainSub = 0xbd,             // - _
        MainAdd = 0xbb,             // + =
        BackSpace = 0x08,           // backspace
        Insert = 0x2d,              // insert
        Home = 0x24,                // home
        PageUp = 0x21,              // page up
        //第三行
        Tab = 0x09,                 // tab
        Q = 0x51,                   // Q
        W = 0x57,                   // W
        E = 0x45,                   // E
        R = 0x52,                   // R
        T = 0x54,                   // T
        Y = 0x59,                   // Y
        U = 0x55,                   // U
        I = 0x49,                   // I
        O = 0x4f,                   // O
        P = 80,                     // P
        P_ = 0xdb,                  // [ {
        P__ = 0xdd,                 // ] }
        P___ = 0xdc,                // \ |
        [Obsolete("use Delete instead")]
        P____ = -1,
        End = 0x23,                 // end
        PageDown = 0x22,            // page down
        //第四行
        CapsLock = 0x14,            // caps lock
        A = 0x41,                   // A
        S = 0x53,                   // S
        D = 0x44,                   // D
        F = 70,                     // F
        G = 0x47,                   // G
        H = 0x48,                   // H
        J = 0x4a,                   // J
        K = 0x4b,                   // K
        L = 0x4c,                   // L
        L_ = 0xba,                  // ; :
        L__ = 0xde,                 // ' "
        [Obsolete("Use Enter instead")]
        L___ = -1,                  // enter
        //第五行
        LeftShift = 0xa0,           // shift
        Z = 90,                     // Z
        X = 0x58,                   // X
        C = 0x43,                   // C
        V = 0x56,                   // V
        B = 0x42,                   // B
        N = 0x4e,                   // N
        M = 0x4d,                   // M
        M_ = 0xbc,                  // , <
        M__ = 0xbe,                 // . >
        M___ = 0xbf,                // / ?
        RightShift = 0xa1,          // shift
        Up = 0x26,                  // up arrow
        //第六行
        LeftCtrl = 0xa2,            // control
        LeftWin = 0x5b,             // win
        LeftAlt = 0xa4,             // alt
        Space = 0x20,               // space
        RightAlt = 0xa5,            // alt
        RightWin = 0x5c,            // win
        App = 0x5d,                 // apps
        RightCtrl = 0xa3,           // control
        Left = 0x25,                // left arrow
        Down = 0x28,                // down arrow
        Right = 0x27,               // right arrow

        #endregion

        #region Keypad

        //第一行
        NumLock = 0x90,             // num
        Divide = 0x6f,              // /
        Multiply = 0x6a,            // *
        Sub = 0x6d,                 // -
        //第二行
        Number7 = 0x67,             // 7
        Number8 = 0x68,             // 8
        Number9 = 0x69,             // 9
        Add = 0x6b,                 // +
        //第三行
        Number4 = 0x64,             // 4
        [Obsolete("It will returns None if the keypad is not locked")]
        Number5 = 0x65,             // 5
        Number6 = 0x66,             // 6
        //第四行
        Number1 = 0x61,             // 小键盘1
        Number2 = 0x62,             // 小键盘2
        Number3 = 0x63,             // 小键盘3
        [Obsolete("Use Enter instead")]
        Number3_ = -1,              // 小键盘回车
        //第六行
        Number0 = 0x60,             // 小键盘0
        [Obsolete("Use Delete if the keypad is not locked")]
        Point = 110,                // . del

        #endregion
    }

    /// <summary>
    /// 输入类 <see langword="static"/>
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// 上一次按下的键
        /// </summary>
        private static KeyCode LastInputKey { get; set; }

        /// <summary>
        /// 上一次松开的键
        /// </summary>
        private static KeyCode LastDropKey { get; set; }

        /// <summary>
        /// 是否在窗口未激活情况下获取输入
        /// </summary>
        public static bool RunInBackground { set; get; }

        /// <summary>
        /// 获取持续的按键输入
        /// </summary>
        public static bool GetKey(KeyCode keyCode)
        {
            if (!RunInBackground && Application.IsBackground)
                return false;

            return (LowLevelAPI.GetAsyncKeyState((int)keyCode) & 0x8000) != 0;
        }

        /// <summary>
        /// 获取按下指定按键
        /// </summary>
        public static bool GetKeyDown(KeyCode keyCode)
        {
            if (!RunInBackground && Application.IsBackground)
                return false;

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
            if (!RunInBackground && Application.IsBackground)
                return false;

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
            if (!RunInBackground && Application.IsBackground)
                return 0;

            int result = 0;
            if (GetKey(negative))
                result -= 1;
            if (GetKey(positive))
                result += 1;

            return result;
        }

        /// <summary>
        /// 获取当前按下的键
        /// </summary>
        public static KeyCode GetCurrentKey()
        {
            if (!RunInBackground && Application.IsBackground)
                return KeyCode.None;

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

        /// <summary>
        /// 获取输入的字符
        /// </summary>
        [Obsolete("Dont suggest using this.")]
        public static char GetInputChar()
        {
            if (!RunInBackground && Application.IsBackground)
                return default(char);

            while (Console.KeyAvailable)
                return Console.ReadKey().KeyChar;
            return default(char);
        }
    }
}