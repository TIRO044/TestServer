using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class SocketListener
    {
        Socket _serverSocket;

        // TODO : 소켓 리스너는 반드시 클라이언트의 입장 부분만 관리하도록 한다.
        // TODO : 액션, 이벤트로 외부에 알려줄 수단을 받고, 연결되면 Invoke해서 클라이언트 소캣을 넘겨줘야 한다.
        // TODO : 그 후에, Recive하는건, 다른 클래스에서 해주도록 하자.
       
        //Action<Socket> _onConnectedAction;

        Func<Session> SessionFactory;

        public SocketListener(Func<Session> action) 
        {
            //_onConnectedAction = action;
            SessionFactory = action;
        }

        public void CreateSocket(EndPoint endPoint)
        {
            _serverSocket = new Socket(endPoint.AddressFamily, socketType: SocketType.Stream, protocolType: ProtocolType.Tcp);
            
            _serverSocket.Bind(endPoint);
            _serverSocket.Listen(backlog: 10);

            // 소캣을 만들어주고
            var connectArgs = new SocketAsyncEventArgs();
            connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnected);
            AcceptClient(connectArgs);

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
            //여기는 이제 별도의 job쓰레드로 돌아가기 때문에, 위험한 지역이다.
            if(e.SocketError == SocketError.Success) {
                Console.WriteLine("Client Connected Success");

                var session = SessionFactory.Invoke();   
                if(session == null) {
                    return;
                }

                session.Start(e.AcceptSocket);

                AcceptClient(e);
            } else {
                Console.WriteLine("Client Connected Fail \n");
            }
        }
    }
}
