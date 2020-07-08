using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        private static SocketListener _serverSocket = new SocketListener();

        static void Main(string[] args)
        {
            _serverSocket.InitSocket();

            while(true) {
            
            }
        }
    }
}
