using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class PacketSession : Session {
        int headerSize = 2;

        public sealed override int OnReceive(ArraySegment<byte> buffer) 
        {
            int processLen = 0;
            while(true) {
                //최소 헤더를 확인할 수 있는 상태인지
                if(buffer.Count < headerSize) {
                    break;
                }

                //완전체로 받았느냐?
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Count);
                if(buffer.Count < dataSize) {
                    break;
                }

                // 패킷을 받았다.
                // ArraySegment는 구조체기 때문에 이런식으로 사용해도 상관이 없다.
                // buffer.Slice라는 걸 사용할 수도 있다.
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

                //받았으면 버퍼를 이동시켜줘라.
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
                processLen += processLen;

            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        private Socket _clientSocket;
        private int _isConnected = 0;

        private SocketAsyncEventArgs _sendArgs;
        private SocketAsyncEventArgs _receiveArgs;

        RecvBuffer _rBuffer = new RecvBuffer(1024);

        // 해야할 것
        // 센드 큐를 만들어야 함
        private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        // 쌓아 두다가. send 이벤트가 불렸을 때 처리가 해야 함
        // lock하는 처리를 해야 함
        private object _sendLock = new object();

        public abstract void OnConnected(EndPoint endpoint);
        public abstract void OnDisConnented(EndPoint endpoint);
        public abstract int OnReceive(ArraySegment<byte> reciveData);
        public abstract void OnSend(int sendData);

        public int IsConnected { get => _isConnected; }

        public void Start(Socket socket)
        {
            _isConnected = 1;

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
            if(Interlocked.Exchange(ref _isConnected, 0) == 0) {
                return;
            }

            OnDisConnented(_clientSocket.RemoteEndPoint);
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        #region Send
        public void SendReuqest(ArraySegment<byte> sendQueue)
        {
            lock (_sendLock) {
                _sendQueue.Enqueue(sendQueue);
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
                    OnSend(args.BytesTransferred);
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
            _rBuffer.CleanUp();
            ArraySegment<byte> segment = _rBuffer.WriteSegment;
            _receiveArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            //소캣을 보내주고, 받고 하는건 커널 딴에서 하는 것이기 때문에 부하가 좀 있다.
            var panding = _clientSocket.ReceiveAsync(_receiveArgs);
            if (!panding) {
                OnReceived(null, _receiveArgs);
            }
        }

        public void OnReceived(object sender, SocketAsyncEventArgs args)
        {
            Console.Write($"received\n");

            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
                try {
                    //받았으면 써야제
                    if(!_rBuffer.OnWirte(args.BytesTransferred)){
                        Disconnect();
                        return;
                    }

                    var read = OnReceive(_rBuffer.ReadSegment);
                    if(read < 0 || _rBuffer.DataSize < read) {
                        Disconnect();
                        return;
                    }

                    if(!_rBuffer.OnRead(read)) {
                        Disconnect();
                        return;
                    }

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
