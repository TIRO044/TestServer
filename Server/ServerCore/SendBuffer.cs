using System;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace ServerCore
{
    public class SendBufferHelper 
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        static int _chunkSize { get; set; } = 4096 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if(CurrentBuffer.Value == null) {
                CurrentBuffer.Value = new SendBuffer(_chunkSize);
            }

            if(reserveSize > CurrentBuffer.Value.FreeSize) {
                CurrentBuffer.Value = new SendBuffer(_chunkSize);
            }

            return CurrentBuffer.Value.OpenSendBuffer(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize) 
        {
            return CurrentBuffer.Value.CloseSendBuffer(usedSize);
        }
    }

    public class SendBuffer
    {
        byte[] _buffer;
        int _usedSize;

        public int FreeSize { get => _buffer.Length - _usedSize; }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }

        //일단 존나 크게 열고,
        public ArraySegment<byte> OpenSendBuffer(int openSize) 
        {
            if(openSize > FreeSize) {
                return null;
            }

            return new ArraySegment<byte>(_buffer, _usedSize, openSize);
        }

        //실제 사용하고나서 실제 사이즈를 가지고 오는 것임
        public ArraySegment<byte> CloseSendBuffer(int usedSize) 
        {
            var target = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return target;
        }
    }
}
