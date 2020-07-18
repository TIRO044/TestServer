using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Session
    {
        private Socket _clientSocket;
        private int _disconnected = 0;

        private SocketAsyncEventArgs _sendArgs;

        // 해야할 것
        // 센드 큐를 만들어야 함
        private Queue<Byte[]> _sendQueue;
        // 쌓아 두다가. send 이벤트가 불렸을 때 처리가 해야 함
        // lock하는 처리를 해야 함
        private object _sendLock;
        // 

        public void Start(Socket socket)
        {
            Console.WriteLine($"Start Session !!");

            _clientSocket = socket;
            SocketAsyncEventArgs completeArgs = new SocketAsyncEventArgs();
            completeArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceived);
            completeArgs.SetBuffer(new byte[1024], 0, 1024);

            RegisterReceive(completeArgs);
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

        #region
        public void SendReuqest(string sendQueue)
        {
            if (string.IsNullOrEmpty(sendQueue)) {
                return;
            }

            _sendQueue.Enqueue(Encoding.UTF8.GetBytes(sendQueue));
        }

        public void Dequeue()
        {
            lock (_sendLock) {
                var sendByte = _sendQueue.Dequeue();

                _sendArgs.Completed += OnSendComplete;
                _sendArgs.SetBuffer(sendByte, 0, sendByte.Length);

                _clientSocket.SendAsync(_sendArgs);
            }
        }


        public void OnSendComplete(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
                if (_sendQueue.Count > 0) {
                    Dequeue();
                }
            } else {
                Console.WriteLine(args.SocketError);
            }
        }

        public void SendUpdate()
        {
            if (_sendQueue.Count > 0) {
                Dequeue();
            }
        }

        #endregion

        #region Recieve
        public void RegisterReceive(SocketAsyncEventArgs arg)
        {
            //소캣을 보내주고, 받고 하는건 커널 딴에서 하는 것이기 때문에 부하가 좀 있다.
            var panding = _clientSocket.ReceiveAsync(arg);
            if (!panding)
            {
                OnReceived(null, arg);
            }
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
        #endregion
    }
}
