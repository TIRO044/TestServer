using System;
using System.ComponentModel;
using System.Net;
using System.Text;
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

                        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                        int lenth = 0;
                        for (int i = 0; i < 3; i++) {
                            var str = $"Send Test 1 ~~ {i} \n";
                            var data = Encoding.UTF8.GetBytes(str);
                            lenth += data.Length;
                            Array.Copy(data, 0, openSegment.Array, openSegment.Offset, data.Length);
                            //오프셋을 더해야하나.
                            //_connectedSession.SendReuqest($"Send Test 1 ~~ {i} \n");
                        }
                        ArraySegment<byte> closeData = SendBufferHelper.Close(lenth);
                        // send 데이터 넘겨준다

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