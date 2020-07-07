using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SocketBinder
    {
        Socket _serverSocket;
        Socket _clientSocket;

        bool _isConnected;

        public Action ReciveAction;
        public Action SendAction;

        public void InitSocket()
        {
            //호스트를 가져온다 (내껄로)

            // 소캣을 만들어주고
            CreateSocket();

            AcceptClient();

            ReciveAction = ReciveRun;
            // Bind해준다. (소캣을)

            // 클라이언트 접속을 기다린다. (Accept)

            // 받고

            // 보낸다.
        }

        public void Update()
        {
            
        }

        public void CreateSocket()
        {
            var myHost = Dns.GetHostName();
            var myHostEntity = Dns.GetHostEntry(myHost);
            var address = myHostEntity.AddressList[0];
            var endPoint = new IPEndPoint(address, 7777);

            _serverSocket = new Socket(endPoint.AddressFamily, socketType: SocketType.Stream, protocolType: ProtocolType.Tcp);
            
            _serverSocket.Bind(endPoint);
            _serverSocket.Listen(backlog: 10);
        }

        public void AcceptClient()
        {
            SocketAsyncEventArgs connectEvent = new SocketAsyncEventArgs();
            connectEvent.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnected);

            _serverSocket.AcceptAsync(connectEvent);
        }

        public void OnConnected(object sender, SocketAsyncEventArgs e)
        {
            _clientSocket = e.AcceptSocket;
            _isConnected = true;

            Task.Factory.StartNew(ReciveRun);
            Task.Factory.StartNew(SendTask);
        }

        public void ReciveRun() 
        {
            while (true) {
                if(!_serverSocket.Connected) {
                    break;
                }

                var onReciveEvent = new SocketAsyncEventArgs();
                onReciveEvent.Completed += OnRecive;

                if(!_clientSocket.ReceiveAsync(onReciveEvent)) {
                    Thread.Sleep(10);
                }
            }
        }

        public void OnRecive(object sender, SocketAsyncEventArgs e) 
        {
            var reciveBuffer = e.Buffer;

        }

        public void SendTask() { 
        
        }
    }
}
