using System;

namespace MongoDB.Infrastructure
{
    public class MongoDbKeepAliveSettings
    {
        public uint OnOff { get; set; }
        public uint KeepAliveTime { get; set; }
        public uint KeepAliveInterval { get; set; }

        public byte[] ToBytes()
        {
            var bytes = new byte[12];
            Array.Copy(BitConverter.GetBytes(OnOff), 0, bytes, 0, 4);
            Array.Copy(BitConverter.GetBytes(KeepAliveTime), 0, bytes, 4, 4);
            Array.Copy(BitConverter.GetBytes(KeepAliveInterval), 0, bytes, 8, 4);
            return bytes;
        }
    }
}
