using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Net.IO
{
    internal class PacketReader : BinaryReader
    {
        private NetworkStream _stream;
        public PacketReader(NetworkStream stream) : base(stream)
        {
            _stream = stream;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            _stream.Read(msgBuffer, 0, length);

            var msg = Encoding.ASCII.GetString(msgBuffer);

            return msg;
        }
    }
}
