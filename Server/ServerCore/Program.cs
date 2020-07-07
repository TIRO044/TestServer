using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Dns.GetHostName();
            var ipHost = Dns.GetHostEntry(host);
            var address = ipHost.AddressList[0];
            var endPoint = new IPEndPoint(address, 7777);

            // 문지기
            Socket listenSocket = new Socket(endPoint.AddressFamily, socketType: SocketType.Stream, protocolType: ProtocolType.Tcp);

            //문지기 교육
            listenSocket.Bind(endPoint);

            //backlog : 최대 대기 수
            listenSocket.Listen(10);    
        
            while(true) {

                //클라이언트의 소켓이다 (연결 되었을 때)
                Socket clientSocket = listenSocket.Accept();

                //연결 되었으면 받는다.

                byte[] recvBuff = new byte[1024];
                int recvBytes = clientSocket.Receive(recvBuff);
                var data = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"recive {data}");

                byte[] sendBuff = Encoding.UTF8.GetBytes("Hi Client");
                clientSocket.Send(sendBuff);

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }
}
