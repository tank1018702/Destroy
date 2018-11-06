namespace Boxhead.Message
{
    using System;
    using System.Collections.Generic;

    public enum ActionType
    {
        Client,
        Server,
    }

    public enum MessageType
    {
        None,
        //Server
        FrameSync,
        StartGame,
        //Client
        PlayerInput,
    }

    [Serializable]
    public class PlayerInput
    {
        public int frameIndex;          // 4bytes
        public bool up;                 // 1byte
        public bool down;               // 1byte
        public bool left;               // 1byte
        public bool right;              // 1byte
    }

    [Serializable]
    public class FrameSync
    {
        public int frameIndex;                  // 4bytes server current frame
        public List<PlayerInput> playerInputs;  // nbytes all playerInputs
    }

    [Serializable]
    public class StartGame
    {
        public int id;                  // 4bytes
    }
}