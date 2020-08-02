using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {

        static Socket _clientSocket;
        static bool receiveStart = false;
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
                if(Interlocked.CompareExchange(ref _connected, desired, expected) == expected){
                    Console.WriteLine($"reciveStart ..");
                    SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
                    receiveStart = true;
                    arg.Completed += OnRecive;
                    arg.SetBuffer(new byte[1024], 0 , 1024);
                    ClientReceiver(arg);
                }

                //SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
                //arg.Completed += OnRecive;
                //ClientReciver(arg);
            } else {
                Console.WriteLine($"Connected Fail _ {args.SocketError}");
            }
        }

        static void ClientReceiver(SocketAsyncEventArgs args)
        {
            var pending = _clientSocket.ReceiveAsync(args);
            if (!pending) {
                OnRecive(null, args);
            }
        }

        static void OnRecive(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
                var receiveData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                Console.WriteLine($"to server \n {receiveData}");
                
                ClientReceiver(args);
            } else {
                Console.WriteLine("error : " + args.SocketError);
            }
        }
    }
}
