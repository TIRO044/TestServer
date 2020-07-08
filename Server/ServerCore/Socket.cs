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

        public void InitSocket()
        {
            //호스트를 가져온다 (내껄로)

            // 소캣을 만들어주고
            CreateSocket();
            // 클라이언트 연결을 기다린다.
            AcceptClient();
        }

        private void CreateSocket()
        {
            var myHost = Dns.GetHostName();
            var myHostEntity = Dns.GetHostEntry(myHost);
            var address = myHostEntity.AddressList[0];
            var endPoint = new IPEndPoint(address, 7777);

            _serverSocket = new Socket(endPoint.AddressFamily, socketType: SocketType.Stream, protocolType: ProtocolType.Tcp);
            
            _serverSocket.Bind(endPoint);
            _serverSocket.Listen(backlog: 10);

            Console.WriteLine($"Success Create Socket");
        }

        private void AcceptClient()
        {
            SocketAsyncEventArgs connectEvent = new SocketAsyncEventArgs();
            connectEvent.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnected);

            _serverSocket.AcceptAsync(connectEvent);
        }

        private void OnConnected(object sender, SocketAsyncEventArgs e)
        {
            Console.WriteLine("Client Connected Success");
            
            _clientSocket = e.AcceptSocket;

            SocketAsyncEventArgs disConnected = new SocketAsyncEventArgs();
            disConnected.Completed += new EventHandler<SocketAsyncEventArgs>();

            _clientSocket.DisconnectAsync
            _isConnected = true;

            Task.Factory.StartNew(ReciveRun);
            Task.Factory.StartNew(SendTask);
        }

        private void OnDisConnected(object sender, SocketAsyncEventArgs e) 
        {
        
        }

        private void ReciveRun() 
        {
            try {
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
            } catch(Exception e) {
                Console.WriteLine($"recive exception {e}");
            }
        }

        private void OnRecive(object sender, SocketAsyncEventArgs e) 
        {
         
        }

        private void SendTask() { 
        
        }
    }
}
