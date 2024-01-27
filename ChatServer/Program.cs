using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    internal class Program
    {
        static List<Client> _users;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 4444);
            _listener.Start();

            Console.WriteLine("Listening on IP: 127.0.0.1, Port: 4444");

            while(true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                /* Broadcast the connection to everyone on the server*/
            }
        }
    }
}