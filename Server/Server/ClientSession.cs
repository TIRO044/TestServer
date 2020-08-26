using ServerCore;
using System;
using System.Net;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public enum PacketID {
            Dummy,
            PlayerInfo
        }
        
        public class PacketBase 
        {
            public ushort Size;
            public ushort Id;
        }

        public class TestPack : PacketBase
        {
            public long PlayerId;
        }

        public override void OnConnected(EndPoint endPoint) 
        {
            Console.WriteLine($"OnConnected Client _ {endPoint}");

            //보내기 연습
            var testPacket = new TestPack() {
                Id = (ushort)PacketID.PlayerInfo,
                Size = 8,
                PlayerId = 30
            };

            bool success = true;
            ushort size = 0;
            var sendArray = SendBufferHelper.Open(4096);

            size += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(sendArray.Array, sendArray.Offset + size, sendArray.Count - size), testPacket.Id);
            size += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(sendArray.Array, sendArray.Offset + size, sendArray.Count - size), testPacket.PlayerId);
            size += 8;
            success &= BitConverter.TryWriteBytes(new Span<byte>(sendArray.Array, sendArray.Offset, sendArray.Count), size);
            var closeArray = SendBufferHelper.Close(size);

            if (success){
                SendReuqest(closeArray);
            }
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
