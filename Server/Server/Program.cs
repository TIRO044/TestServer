using System;
using System.Net;
using System.Threading;
using ServerCore;

namespace Server
{
    class Program
    {
        private static SocketListener _listener;
        private static Session _connectedSession;
        
        static void Main(string[] args)
        {
            var myHost = Dns.GetHostName();
            var myHostEntity = Dns.GetHostEntry(myHost);
            var address = myHostEntity.AddressList[0];
            var endPoint = new IPEndPoint(address, 7777);

            _listener = new SocketListener(() => {
                if (_connectedSession != null && _connectedSession.IsConnected == 1) {
                    _connectedSession.Disconnect();
                }

                _connectedSession = new GameSession();
                return _connectedSession;
            });

            _listener.CreateSocket(endPoint);

            while (true) {
                try {
                    if (_connectedSession != null && _connectedSession.IsConnected == 1) {
                        Thread.Sleep(500);
                        for (int i = 0; i < 3; i++) {
                            _connectedSession.SendReuqest($"Send Test 1 ~~ {i} \n");
                        }

                        for (int i = 0; i < 3; i++) {
                            _connectedSession.SendReuqest($"Send Test 2 !! {i} \n");

                        }
                        //_sendTest = true;

                        Thread.Sleep(500);
                        _connectedSession?.SendUpdate();
                    }
                } catch (Exception e) {
                    Console.Write(e);
                }
            }
        }
    }
}