using ChatServer.Net.IO;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    internal class Program
    {
        // List to store connected clients.
        static List<Client> _users;
        // TcpListener for listening to incoming connections.
        static TcpListener _listener;
        static void Main(string[] args)
        {
            // Initialize the list of clients and start the listener on the specified IP address and port.
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 4444);
            _listener.Start();

            Console.WriteLine("Listening on IP: 127.0.0.1, Port: 4444");

            while(true)
            {
                // Accept a new client connection and create a Client object to handle it.
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                /* Broadcast the connection to everyone on the server*/
                BroadcastConnection();

            }
        }

        static void BroadcastConnection()
        {
            foreach(var user in  _users)
            {
                foreach(var usr in _users)
                {
                    // Construct a broadcast packet containing the username and UID of each connected user.
                    var broadcastpacket = new PacketBuilder();
                    broadcastpacket.WriteOpCode(1);
                    broadcastpacket.WriteMessage(usr.Username);
                    broadcastpacket.WriteMessage(usr.UID.ToString());

                    // Send the broadcast packet to the current user's client socket.
                    user.ClientSocket.Client.Send(broadcastpacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach(var user in _users)
            {
                // Construct a message packet containing the message to be broadcast.
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);

                // Send the message packet to the current user's client socket.
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }
        public static void BroadcastDisconnect(string uid)
        {
            // Find the disconnected user in the list of clients and remove them.
            var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);
            
            foreach (var user in _users)
            {
                // Construct a broadcast packet containing the UID of the disconnected user.
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);

                // Send the broadcast packet to each connected client.
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            // Broadcast a disconnection message to all connected clients.
            BroadcastMessage($"[{disconnectedUser.Username}]: Disconnected!");
        }
    }
}