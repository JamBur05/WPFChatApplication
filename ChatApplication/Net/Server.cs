using ChatClient.Net.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    internal class Server
    {

        TcpClient _client;
        PacketBuilder _packetBuilder;
        public PacketReader PacketReader;
        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action userDisconnectEvent;
        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            // Check if the client is not already connected.
            if (!_client.Connected)
            {
                // Attempt to connect to the server at the specified IP address and port.
                _client.Connect("127.0.0.1", 4444);
                PacketReader = new PacketReader(_client.GetStream());

                if(!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    
                    // OpCode 0 indicates a connection request.
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }

                // Start reading packets from the server in a separate task.
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            // Start a new task to continuously read packets from the server.
            Task.Run(() =>
            {
                while (true)
                {
                    // Read the opcode from the packet.
                    var opcode = PacketReader.ReadByte();

                    // Process the opcode and invoke corresponding events.
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("Ah yes..");
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            // Construct a packet for sending the message to the server.
            var messagePacket = new PacketBuilder();
            
            // OpCode 5 indicates a message.
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
