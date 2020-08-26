using System;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    class Program
    {
        private static SocketListener _listener;
        private static Session _clientSession;
        
        static void Main(string[] args)
        {
            var myHost = Dns.GetHostName();
            var myHostEntity = Dns.GetHostEntry(myHost);
            var address = myHostEntity.AddressList[0];
            var endPoint = new IPEndPoint(address, 7777);

            _listener = new SocketListener(() => {
                if (_clientSession != null && _clientSession.IsConnected == 1) {
                    _clientSession.Disconnect();
                }

                _clientSession = new ClientSession();
                return _clientSession;
            });

            _listener.CreateSocket(endPoint);

            while (true) {
                try {
                    if (_clientSession != null && _clientSession.IsConnected == 1) {
                        Thread.Sleep(5000);

                        //for (int i = 0; i < 3; i++) {
                        //    var str = $"Send Test ~~ {i} \n";
                        //    var data = Encoding.UTF8.GetBytes(str);
                        //    ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                        //    Array.Copy(data, 0, openSegment.Array, openSegment.Offset, data.Length);
                        //    ArraySegment<byte> closeData = SendBufferHelper.Close(data.Length);
                        //    _clientSession.SendReuqest(closeData);
                        //}
                     
                        Thread.Sleep(500);
                        _clientSession?.SendUpdate();
                    }
                } catch (Exception e) {
                    Console.Write(e);
                }
            }
        }
    }
}