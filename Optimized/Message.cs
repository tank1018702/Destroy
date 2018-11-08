using System.IO;
using System.Text;
using UnityEngine;
using PBMessage;
using System;
using System.Collections.Generic;

namespace Message
{
    /// <summary>
    /// 数据包的封装与拆包 <see langword="static"/>
    /// </summary>
    public static class MsgConverter
    {
        public const int MSG_HEAD = 2; //包头长度

        public static int GetHead(byte[] head)
        {
            using (MemoryStream stream = new MemoryStream(head))
            {
                BinaryReader binary = new BinaryReader(stream, Encoding.UTF8);
                return binary.ReadUInt16();
            }
        }

        /// <summary>
        /// 需要根据项目不同重写
        /// </summary>
        public static byte[] Pack(Msg msg, uint timeStamp, byte[] data = null)
        {
            List<byte> bytes = new List<byte>();
            //不为空, 并且长度大于0
            if (data != null && data.Length > 0)
            {
                //包头
                bytes.AddRange(BitConverter.GetBytes((ushort)(10 + data.Length)));  //2-消息总长度
                //包体
                bytes.AddRange(BitConverter.GetBytes((uint)timeStamp));             //4-时间戳
                bytes.AddRange(BitConverter.GetBytes((ushort)msg.Act));             //2-发送方
                bytes.AddRange(BitConverter.GetBytes((ushort)msg.Cmd));             //2-消息类型
                bytes.AddRange(data);                                               //n-消息内容
            }
            else
            {
                //包头
                bytes.AddRange(BitConverter.GetBytes((ushort)10));                  //2-消息总长度
                //包体
                bytes.AddRange(BitConverter.GetBytes((uint)timeStamp));             //4-时间戳
                bytes.AddRange(BitConverter.GetBytes((ushort)msg.Act));             //2-发送方
                bytes.AddRange(BitConverter.GetBytes((ushort)msg.Cmd));             //2-消息类型
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// 需要根据项目不同重写
        /// </summary>
        public static Msg UnPack(byte[] body, out uint timeStamp, out byte[] data)
        {
            ActType act;
            CmdType cmd;
            using (MemoryStream stream = new MemoryStream(body))
            {
                BinaryReader binary = new BinaryReader(stream, Encoding.UTF8);

                timeStamp = binary.ReadUInt32();
                act = (ActType)binary.ReadUInt16();
                cmd = (CmdType)binary.ReadUInt16();

                //排除已经读取的8个字节
                int dataLength = body.Length - 8;

                //有长度就读出字节数组来
                if (dataLength > 0)
                    data = binary.ReadBytes(dataLength);
                //data长度为0就新建长度为0的数组, 必须保证数组不能为null
                else
                    data = new byte[0];
            }
            return new Msg(act, cmd);
        }
    }

    public struct Msg
    {
        public ActType Act;
        public CmdType Cmd;

        public Msg(ActType act, CmdType cmd)
        {
            Act = act;
            Cmd = cmd;
        }
    }

    public enum ActType
    {
        Request,     //C2S
        BroadCast,   //C2S(要求服务器广播所有客户端)

        Response,    //S2C
    }

    public enum CmdType
    {
        HeartBeat,      //心跳包(没有包体)

        Connect,        //服务器在客户端上线给客户端发送的信息
        Online,         //上线
        EditInfo,       //编辑信息

        Spawn,          //出生
        Move,           //移动

        CreatRoom,      //创建房间
        EnterRoom,      //进入房间
        StartGame,      //开始游戏
    }

    public static class TimeConverter
    {
        public static uint Float2UInt(float timeStamp)
        {
            return (uint)(timeStamp * 1000);
        }

        public static float UInt2Float(uint timeStamp)
        {
            return (float)timeStamp / 1000;
        }
    }

    public static class VectorConverter
    {
        public static Vec3 ToVec3(this Vector3 vector3)
        {
            Vec3 vec3 = new Vec3();
            vec3.X = (int)(vector3.x * 1000);
            vec3.Y = (int)(vector3.y * 1000);
            vec3.Z = (int)(vector3.z * 1000);
            return vec3;
        }

        public static Vector3 ToVector3(this Vec3 vec3)
        {
            Vector3 vector3 = new Vector3();
            vector3.x = (float)vec3.X / 1000;
            vector3.y = (float)vec3.Y / 1000;
            vector3.z = (float)vec3.Z / 1000;
            return vector3;
        }
    }

    public static class TransformConverter
    {
        public static PBMessage.Transform ToPBTransform(this UnityEngine.Transform transform)
        {
            PBMessage.Transform t = new PBMessage.Transform();
            t.Position = transform.position.ToVec3();
            t.Rotation = transform.eulerAngles.ToVec3();
            t.Scale = transform.localScale.ToVec3();
            return t;
        }

        public static void ToTransform(this PBMessage.Transform pbTransform, UnityEngine.Transform transform)
        {
            transform.position = pbTransform.Position.ToVector3();
            transform.eulerAngles = pbTransform.Rotation.ToVector3();
            transform.localScale = pbTransform.Scale.ToVector3();
        }
    }
}