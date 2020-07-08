using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        private static SocketBinder _serverSocket = new SocketBinder();

        static void Main(string[] args)
        {
            _serverSocket.InitSocket();

            while(true) {
            
            }
        }
    }
}
