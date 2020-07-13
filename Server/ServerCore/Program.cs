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
            // 여기서 불리는 callback은 별도의 쓰레드 풀에서 돈다.
            _serverSocket.InitSocket();

            while(true) {
            
            }
        }
    }
}
