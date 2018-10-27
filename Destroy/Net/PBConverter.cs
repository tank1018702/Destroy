namespace Destroy.Net
{
#if UseProtobuf
    using System;
    using Google.Protobuf;

    public static class PBConverter
    {
        public static byte[] Serializer<T>(T obj) where T : IMessage
        {
            try
            {
                byte[] data = obj.ToByteArray();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T Deserializer<T>(byte[] data) where T : IMessage, new()
        {
            IMessage message = new T();
            try
            {
                T msg = (T)message.Descriptor.Parser.ParseFrom(data);
                return msg;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
#endif
}