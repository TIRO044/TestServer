using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        private static SocketListener _listener = new SocketListener();

        private static int _clientConnected = -1;
        private static bool _sendTest;

        static Session _connectedSession;

        static void Main(string[] args)
        {
            // 여기서 불리는 callback은 별도의 쓰레드 풀에서 돈다.
            _listener.InitSocket(() => {
                if(_connectedSession != null && _connectedSession.IsConnected == 1){
                    _connectedSession.Disconnect();
                }

                _connectedSession = new GameSession();
                return _connectedSession; 
            });

            while (true) {
                try {
                    if (_connectedSession != null && _connectedSession.IsConnected == 1) {
                        Thread.Sleep(500);
                        if (!_sendTest) {
                            for (int i = 0; i < 3; i++) {
                                _connectedSession.SendReuqest($"Send Test 1 ~~ {i} \n");
                            }

                            for (int i = 0; i < 3; i++) {
                                _connectedSession.SendReuqest($"Send Test 2 !! {i} \n");

                            }
                            //_sendTest = true;
                        }

                        Thread.Sleep(500);
                        _connectedSession?.SendUpdate();
                    }
                } catch(Exception e) {
                    Console.Write(e);
                }
            }
        }
    }
}
