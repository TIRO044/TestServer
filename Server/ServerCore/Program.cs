using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        private static SocketListener _listener = new SocketListener();
        private static Session _clientSession;

        private static int _clientConnected = -1;
        private static bool _sendTest;

        static void Main(string[] args)
        {
            // 여기서 불리는 callback은 별도의 쓰레드 풀에서 돈다.
            _listener.InitSocket(OnConnected);

            while (true) {
                try {
                    if (_clientConnected == 1) {
                        Thread.Sleep(500);
                        if (!_sendTest) {
                            for (int i = 0; i < 3; i++) {
                                _clientSession.SendReuqest($"Send Test 1 ~~ {i} \n");
                            }

                            for (int i = 0; i < 3; i++) {
                                _clientSession.SendReuqest($"Send Test 2 !! {i} \n");

                            }
                            _sendTest = true;
                        }

                        Thread.Sleep(500);
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
        }
    }
}
