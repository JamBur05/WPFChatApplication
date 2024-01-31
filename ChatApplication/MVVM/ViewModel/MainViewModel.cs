using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ChatClient.MVVM.ViewModel
{
    internal class MainViewModel
    {
        // Command properties for connecting to the server and sending messages.
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        // Collections for storing users and messages.
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        private Server _server;
        // Properties for storing message and username.
        public string Message { get; set; }
        public string Username { get; set; }

        public MainViewModel()
        {
            // Initialize collections.
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();

            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += RemoveUser;
            
            // Initialize commands for connecting to the server and sending messages.
            ConnectToServerCommand = new RelayCommand(a => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(a => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));

        }

        private void MessageReceived()
        {
            // Read the message from the server and add it to the Messages collection.
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        private void RemoveUser()
        {
            // Read the user's ID from the server and remove the corresponding user from the Users collection.
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void UserConnected()
        {
            // Read the username and UID of the connected user from the server.
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),
            };

            // Add the user to the Users collection if they are not already present.
            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
    }
}
