using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Dns.GetHostName();
            var ipHost = Dns.GetHostEntry(host);
            var endPoint = new IPEndPoint(ipHost.AddressList[0], 7777);

            Socket clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // 서버에 연결
            clientSocket.Connect(endPoint);

            var server = Encoding.UTF8.GetBytes("Hi Sever");
            clientSocket.Send(server);

            byte[] reciveBuffer = new byte[1024];
            var reciveByte = clientSocket.Receive(reciveBuffer);
            var str = Encoding.UTF8.GetString(reciveBuffer, 0, reciveByte);

            Console.WriteLine($"im client {str}");

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}
