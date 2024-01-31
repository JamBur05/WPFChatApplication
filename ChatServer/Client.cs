using ChatServer.Net.IO;
using System.Net.Sockets;

namespace ChatServer
{
    internal class Client
    {
        // Properties to store client information.
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        PacketReader _packetReader;

        public Client(TcpClient client) 
        {
            // Assign the TcpClient and generate a unique identifier for the client.
            ClientSocket = client;
            UID = Guid.NewGuid();

            // Initialize PacketReader for reading packets from the client's stream.
            _packetReader = new PacketReader(ClientSocket.GetStream());

            // Read the initial packet from the client containing the username.
            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();


            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username {Username}");

            // Start a new task to handle client processing.
            Task.Run(() => Process());
        }
        
        void Process()
        {
            while (true)
            {
                try
                {
                    // Read the opcode from the packet.
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            // If the opcode indicates a message, read the message from the packet and broadcast it to all clients.
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message received! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            break;
                    }
                }
                catch (Exception)
                {
                    // Handle the exception if the client disconnects unexpectedly.
                    Console.WriteLine($"[{UID.ToString()}]: Disconnected!");
                    Program.BroadcastDisconnect(UID.ToString());
                    
                    // Close the client socket.
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
