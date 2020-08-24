using System;

namespace ServerCore
{
    // 버퍼를 Read, Write 위치를 나워서 작업해야 한다.
    // 최적화 때문인가.. ? 정확하겐 모르겠다
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;

        public RecvBuffer(int bufferSize) 
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize );
        }

        public int DataSize { get => _writePos - _readPos; }
        public int FreeSize { get => _buffer.Count - _writePos; }

        // 읽어야 할 부분
        public ArraySegment<byte> ReadSegment { get => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        
        // 앞으로 쓸 수 있는 부분
        public ArraySegment<byte> WriteSegment {  get => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }

        public void CleanUp() 
        {
            if(FreeSize <= 0) {
                _readPos = _writePos = 0;
                //초기화
                return;
            } else {
                // a를 dataSize 만큼 처음으로 옮겼다.
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, DataSize);
                _writePos = DataSize;
                _readPos = 0;
            }   
        }

        public bool OnRead(int numOfByte)
        {
            if(numOfByte > DataSize) {
                return false;
            }

            _readPos += numOfByte;

            return true;
        }

        public bool OnWirte(int numOfSize) 
        {
            if(numOfSize > FreeSize) {
                return false;
            }

            _writePos += numOfSize;

            return true;
        }
    }
}
