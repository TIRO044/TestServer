using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Session
    {
        Socket _clientSocket;
        int _disconnected = 0;

        public void Init(Socket socket)
        {
            _clientSocket = socket;
            SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
            arg.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceived);
            arg.SetBuffer(new byte[1024], 0, 1024);

            StartReceive(arg);
        }

        public void StartReceive(SocketAsyncEventArgs arg) 
        {
            var panding = _clientSocket.ReceiveAsync(arg);
            if(panding){
                OnReceived(null, arg);
            }
        }

        public void DisConnect() 
        {
            // DisConnect를 호출하는 부분이 쓰레드라면, 인터락을 걸어야 함.
            if(Interlocked.Exchange(ref _disconnected, 1) == 1) {
                return;
            }

            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public void OnReceived(object sender, SocketAsyncEventArgs e)
        {
            if(e.BytesTransferred > 0 && e.SocketError == SocketError.Success) { 
            } else { 
            
            }
        }
    }
}
