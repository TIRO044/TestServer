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
        private SocketAsyncEventArgs _receiveArgs;

        // 해야할 것
        // 센드 큐를 만들어야 함
        private Queue<byte[]> _sendQueue = new Queue<byte[]>();
        // 쌓아 두다가. send 이벤트가 불렸을 때 처리가 해야 함
        // lock하는 처리를 해야 함
        private object _sendLock = new object();
        // 

        public void Start(Socket socket)
        {
            Console.WriteLine($"Start Session !!");

            _clientSocket = socket;
            _receiveArgs = new SocketAsyncEventArgs();
            _receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceived);
            _receiveArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += OnSendComplete;
      
            RegisterReceive();
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
            lock (_sendLock) {
                if (string.IsNullOrEmpty(sendQueue)) {
                    return;
                }

                _sendQueue.Enqueue(Encoding.UTF8.GetBytes(sendQueue));
            }
        }

        public void Dequeue()
        {
            lock (_sendLock) {
                if(_sendQueue.Count == 0) {
                    return;
                }

                var sendBufferList = new List<ArraySegment<byte>>();

                while(_sendQueue.Count > 0) {
                    var sendBufferTarget = _sendQueue.Dequeue();
                    sendBufferList.Add(sendBufferTarget);
                }

                _sendArgs.BufferList = sendBufferList;
                _clientSocket.SendAsync(_sendArgs);
            }
        }

        public void OnSendComplete(object sender, SocketAsyncEventArgs args)
        {
            lock (_sendLock) {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
                    Dequeue();
                } else {
                    Console.WriteLine(args.SocketError);
                }
            }
        }

        public void SendUpdate()
        {
            Dequeue();
        }

        #endregion

        #region Recieve
        public void RegisterReceive()
        {
            //소캣을 보내주고, 받고 하는건 커널 딴에서 하는 것이기 때문에 부하가 좀 있다.
            var panding = _clientSocket.ReceiveAsync(_receiveArgs);
            if (!panding)
            {
                OnReceived(null, _receiveArgs);
            }
        }

        public void OnReceived(object sender, SocketAsyncEventArgs args)
        {
            Console.Write($"received\n");

            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
                try {
                    var receivedData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine(receivedData);

                    //_receiveArgs.clear
                    RegisterReceive();
                } catch(Exception e) {
                    Console.Write($"{e} _ receive fail");
                }
            } else {
                Console.Write($"{args.SocketError} _ receive fail");
            }
        }
        #endregion
    }
}
