using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatClient.Net.IO
{
    class PacketReader : BinaryReader
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
            // Read the message data from the network stream into the buffer.
            _stream.Read(msgBuffer, 0, length);
            
            // Convert the message data from bytes to a string using ASCII encoding.
            var msg = Encoding.ASCII.GetString(msgBuffer);

            return msg;
        }
    }
}

