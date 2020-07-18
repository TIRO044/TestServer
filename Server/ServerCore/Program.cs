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

        private static bool _clientConnected = false;
        private static bool _sendTest;

        static void Main(string[] args)
        {
            // 여기서 불리는 callback은 별도의 쓰레드 풀에서 돈다.
            _serverSocket.InitSocket(OnConnected);

            while (true) {
                if (_clientConnected) {
                    if (!_sendTest) {
                        for (int i = 0; i < 3; i ++) {
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
            }
        }

        public static void OnConnected(Socket _clientSocket) 
        {
            _clientConnected = true;

            var session = new Session();
            session.Start(_clientSocket);

            Thread.Sleep(2000);

            session.Disconnect();
        }

        public static void OnDisConnected()
        {
            _clientSession = null;
            _clientConnected = false;
        }
    }
}
