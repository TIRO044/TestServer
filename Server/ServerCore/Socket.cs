using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SocketListener
    {
        Socket _serverSocket;
        Socket _clientSocket;

        SocketAsyncEventArgs OnConnectedAction;

        // TODO : 소켓 리스너는 반드시 클라이언트의 입장 부분만 관리하도록 한다.
        // TODO : 액션, 이벤트로 외부에 알려줄 수단을 받고, 연결되면 Invoke해서 클라이언트 소캣을 넘겨줘야 한다.
        // TODO : 그 후에, Recive하는건, 다른 클래스에서 해주도록 하자.

        public void InitSocket(SocketAsyncEventArgs e)
        {
            //호스트를 가져온다 (내껄로)

            // 소캣을 만들어주고
            CreateSocket();
            // 클라이언트 연결을 기다린다.

            SocketAsyncEventArgs connectEvent = new SocketAsyncEventArgs();
            connectEvent.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnected);

            AcceptClient();ㄴㄴ
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

        private void AcceptClient(SocketAsyncEventArgs e)
        {
            // 이벤트를 재사용하기 때문에, 두번 이상 연결되면 이전에 연결된 소캣임 null로 비워야 한다.
            e.AcceptSocket = null;

            bool pending = _serverSocket.AcceptAsync(e);
            
            //던지자 마자 연결이 되었을 때
            if(!pending) {
                OnConnected(null, e);
            }
        }

        private void OnConnected(object sender, SocketAsyncEventArgs e)
        {
            if(e.SocketError == SocketError.Success) {
                Console.WriteLine("Client Connected Success");

                _clientSocket = e.AcceptSocket;

                SocketAsyncEventArgs connectEvent = new SocketAsyncEventArgs();
                connectEvent.Completed += new EventHandler<SocketAsyncEventArgs>(OnDisConnected);
                _clientSocket.DisconnectAsync(connectEvent);

                Task.Factory.StartNew(ReciveRun);
                Task.Factory.StartNew(SendTask);
            } else {
                Console.WriteLine("Client Connected Fail");
            }

            AcceptClient(e);
        }

        private void OnDisConnected(object sender, SocketAsyncEventArgs e) 
        {
            Console.WriteLine("Client Disconnected");
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
