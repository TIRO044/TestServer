using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Session
    {
        Socket _clientSocket;
        int _disconnected = 0;

        public void Start(Socket socket)
        {
            Console.WriteLine($"Start Session !!");

            _clientSocket = socket;
            SocketAsyncEventArgs completeArgs = new SocketAsyncEventArgs();
            completeArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceived);
            completeArgs.SetBuffer(new byte[1024], 0, 1024);

            RegisterReceive(completeArgs);
        }

        public void RegisterReceive(SocketAsyncEventArgs arg) 
        {
            //소캣을 보내주고, 받고 하는건 커널 딴에서 하는 것이기 때문에 부하가 좀 있다.
            var panding = _clientSocket.ReceiveAsync(arg);
            if(!panding){
                OnReceived(null, arg);
            }
        }

        public void Disconnect() 
        {
            // DisConnect를 호출하는 부분이 쓰레드라면, 인터락을 걸어야 함.
            if(Interlocked.Exchange(ref _disconnected, 1) == 1) {
                return;
            }

            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public void OnReceived(object sender, SocketAsyncEventArgs args)
        {
            Console.Write($"received");

            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
                try {
                    var receivedData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine(receivedData);

                    RegisterReceive(args);
                } catch(Exception e) {
                    Console.Write($"{e} _ receive fail");
                }
            }
        }
    }
}
