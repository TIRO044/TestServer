using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class Program
    {
        static Socket _clientSocket;
        static ServerSession _session;
        static int _connected = -1;
        static void Main(string[] args)
        {
            var host = Dns.GetHostName();
            var ipHost = Dns.GetHostEntry(host);
            var endPoint = new IPEndPoint(ipHost.AddressList[0], 7777);

            // 서버에 연결
            SocketAsyncEventArgs ConncetArg = new SocketAsyncEventArgs();
            ConncetArg.Completed += OnConnected;
            ConncetArg.RemoteEndPoint = endPoint;
            ConncetArg.UserToken = _clientSocket;

            Thread.Sleep(2000);

            _clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.ConnectAsync(ConncetArg);

            while (true) {
                try
                {  
                    Thread.Sleep(100);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }

        static void OnConnected(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success) {
                Console.WriteLine($"Connected Complete");

                var desired = 1;
                var expected = -1;
                if(Interlocked.CompareExchange(ref _connected, desired, expected) == expected) {
                    _session = new ServerSession();
                    Console.WriteLine($"reciveStart .. \n");
                    _session.Start(args.ConnectSocket);
                    _session.OnConnected(args.RemoteEndPoint);
                }
            } else {
                Console.WriteLine($"Connected Fail _ {args.SocketError}");
            }
        }
    }
}
