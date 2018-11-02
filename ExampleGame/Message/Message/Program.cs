namespace Boxhead.Message
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 用于客户端向服务器发送每帧的消息
    /// </summary>
    [Serializable]
    public class C2S_InputMsg
    {
        public int frameIndex;          // 4bytes
        public bool up;                 // 1byte
        public bool down;               // 1byte
        public bool left;               // 1byte
        public bool right;              // 1byte
    }

    /// <summary>
    /// 用于服务器给客户端同步所有玩家的操作
    /// </summary>
    [Serializable]
    public class S2C_FrameSync
    {
        public int frameIndex;                  // 4bytes server current frame
        public List<C2S_InputMsg> playerInputs; // nbytes all playerInputs
    }

    /// <summary>
    /// 用于服务器给客户端发送开始游戏指令
    /// </summary>
    [Serializable]
    public class S2C_StartGameMsg
    {
        public int id;                  // 4bytes
    }
}