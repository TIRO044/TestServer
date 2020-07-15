using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Dns.GetHostName();
            var ipHost = Dns.GetHostEntry(host);
            var endPoint = new IPEndPoint(ipHost.AddressList[0], 7777);

            // 서버에 연결
            while(true) {
                Socket clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try {
                    clientSocket.Connect(endPoint);

                    for (int i = 0; i < 5; i++) {
                        var server = Encoding.UTF8.GetBytes($"Hi Sever {i}");
                        clientSocket.Send(server);
                    }

                    //Thread.Sleep(2000);
                    //byte[] reciveBuffer = new byte[1024];
                    //var reciveByte = clientSocket.Receive(reciveBuffer);
                    //var str = Encoding.UTF8.GetString(reciveBuffer, 0, reciveByte);

                    //Console.WriteLine($"im client {str}");

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();

                    Thread.Sleep(1000);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
