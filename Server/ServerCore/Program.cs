using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        private static SocketListener _serverSocket = new SocketListener();
        private static Session _clientSession;

        private static int _clientConnected = -1;
        private static bool _sendTest;

        static void Main(string[] args)
        {
            // 여기서 불리는 callback은 별도의 쓰레드 풀에서 돈다.
            _serverSocket.InitSocket(OnConnected);

            while (true) {
                try {
                    if (_clientConnected == 1) {
                        Thread.Sleep(5000);

                        if (!_sendTest) {
                            for (int i = 0; i < 3; i++) {
                                _clientSession.SendReuqest($"Send Test 1 ~~ {i} \n");
                            }

                            Thread.Sleep(100);
                            _clientSession?.SendUpdate();

                            for (int i = 0; i < 3; i++) {
                                _clientSession.SendReuqest($"Send Test 2 !! {i} \n");

                            }

                            Thread.Sleep(100);

                            _sendTest = true;
                        }

                        Thread.Sleep(1000);
                        _clientSession?.SendUpdate();
                    }
                } catch(Exception e) {
                    Console.Write(e);
                }
            }
        }

        public static void OnConnected(Socket _clientSocket) 
        {
            var desired = 1;
            var expected = -1;
            
            if (Interlocked.CompareExchange(ref _clientConnected, desired, expected) == expected) {
                _clientSession = new Session();
                _clientSession.Start(_clientSocket);
            }

            //Thread.Sleep(2000);
            //session.Disconnect();
        }

        public static void OnDisConnected()
        {
            var desired = -1;
            var expected = 1;

            if (Interlocked.CompareExchange(ref _clientConnected, desired, expected) == expected) {
                _clientSession = null;
            }
        }
    }
}
