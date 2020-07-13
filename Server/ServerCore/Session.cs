using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Session
    {
        Socket _clientSocket;

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
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public void OnReceived(object sender, SocketAsyncEventArgs e)
        {
            if(e.BytesTransferred > 0 && e.SocketError == SocketError.Success) { 
                e.
            } else { 
            
            }
        }
    }
}
