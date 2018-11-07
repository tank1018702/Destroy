namespace Destroy.Net
{
#if UseProtobuf
    using System;
    using Google.Protobuf;

    public static class PBConverter
    {
        public static byte[] Serializer<T>(T obj) where T : IMessage
        {
            byte[] data = obj.ToByteArray();
            return data;
        }

        public static T Deserializer<T>(byte[] data) where T : IMessage, new()
        {
            IMessage message = new T();
            T msg = (T)message.Descriptor.Parser.ParseFrom(data);
            return msg;
        }
    }
#endif
}