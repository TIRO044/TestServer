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
                //_clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    if(_connected == -1) {
                        continue;
                    }
                    //_clientSocket.Connect(endPoint);

                    //if (!_clientSocket.Blocking) {
                    //    Console.WriteLine($"client blocking..");
                    //    continue;
                    //}

                    //if (!_clientSocket.Connected)
                    //{
                    //    _clientSocket.Connect(endPoint);
                    //    Console.WriteLine($"connected..");
                    //    continue;
                    //}


                    if (!receiveStart) {
                       
                        continue;
                    }


                    Thread.Sleep(10000);
                    //for (int i = 0; i < 5; i++) {
                    //    var server = Encoding.UTF8.GetBytes($"Hi Sever {i}");
                    //    clientSocket.Send(server);
                    //}

                    //Thread.Sleep(2000);
                    //byte[] reciveBuffer = new byte[1024];
                    //var reciveByte = clientSocket.Receive(reciveBuffer);
                    //var str = Encoding.UTF8.GetString(reciveBuffer, 0, reciveByte);

                    //Console.WriteLine($"im client {str}");

                    //_clientSocket.Shutdown(SocketShutdown.Both);
                    //_clientSocket.Close();
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
                    ClientReciver(arg);
                }

                //SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
                //arg.Completed += OnRecive;
                //ClientReciver(arg);
            } else {
                Console.WriteLine($"Connected Fail _ {args.SocketError}");
            }
        }

        static void ClientReciver(SocketAsyncEventArgs args)
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
                Console.WriteLine($"to server {receiveData}");
                
                ClientReciver(args);
            } else {
                Console.WriteLine(args.SocketError);
            }
        }
    }
}
