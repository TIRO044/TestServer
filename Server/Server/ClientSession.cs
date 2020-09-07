using ServerCore;
using System;
using System.Net;
using Packet;
using System.Text;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint) 
        {
            Console.WriteLine($"OnConnected Client _ {endPoint}");

            //보내기 연습
            var tp = new TestPack() {
                PlayerId = 30,
                PlayerName = "test"
            };

            var sendArray = SendBufferHelper.Open(4096);
            var span = new Span<byte>(sendArray.Array, sendArray.Offset, sendArray.Count);
            int count = 4;
            if (tp.Write(ref span, ref count, sendArray)) {
                var closeArr = SendBufferHelper.Close(count);
                SendReuqest(closeArr);
            }

            //bool success = true;
            //int size = 0;
            //Span<byte> s = new Span<byte>(sendArray.Array, sendArray.Offset, sendArray.Count);
            //size += sizeof(ushort);
            //success &= BitConverter.TryWriteBytes(s.Slice(size, s.Length - size), 1);
            //size += sizeof(ushort);
            //success &= BitConverter.TryWriteBytes(s.Slice(size, s.Length - size), 2);

            ////문자열은 가변적이기 떄문에 크기를 먼저 구한다.
            ////ushort를 하는 이유는, UTF 16(c#에선 기본) 유니코드 2바이트를 사용하기 때문임
            //ushort strLenth = (ushort)Encoding.Unicode.GetByteCount("tsete");
            //success &= BitConverter.TryWriteBytes(s.Slice(size, s.Length - size), strLenth);
            //size += sizeof(ushort);
            //// ㄴ 크기를 넣었음
            //Array.Copy(Encoding.Unicode.GetBytes("tsete"), 0, sendArray.Array, size, strLenth);
            //size += strLenth;

            //size += sizeof(long);
            //success &= BitConverter.TryWriteBytes(s.Slice(size, sendArray.Count - size), size);
            //var closeArray = SendBufferHelper.Close(size);

            //var lenth = (ushort)Encoding.Unicode.getcount
            //if (success) {
            //    SendReuqest(closeArray);
            //}
        }

        public override void OnDisConnented(EndPoint endPoint)
        {
            Console.WriteLine($"DisConnected {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"Receive Size {size} , id : {id}");
        }

        public override void OnSend(int sendData)
        {
            Console.WriteLine("Send 뭐.. 아직은 별로 필요하진 않음");
        }
    }
}
