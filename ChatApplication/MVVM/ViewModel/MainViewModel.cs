using ChatClient.MVVM.Core;
using ChatClient.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.MVVM.ViewModel
{
    internal class MainViewModel
    {
        public RelayCommand ConnectToServerCommand { get; set; }

        private Server _server;
        public string Username { get; set; }

        public MainViewModel()
        {
            _server = new Server(); 
            ConnectToServerCommand = new RelayCommand(a => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
        }
    }
}
