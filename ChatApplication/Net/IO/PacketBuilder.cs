using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net.IO
{
    class PacketBuilder
    {
        MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }

        public void WriteString(string msg)
        {
            var msgLenghth = msg.Length;
            _ms.Write(BitConverter.GetBytes(msgLenghth));
            _ms.Write(Encoding.ASCII.GetBytes(msg));
        }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
}
