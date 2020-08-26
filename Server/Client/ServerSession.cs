using ServerCore;
using System;
using System.Net;

namespace Client
{
    class ServerSession : PacketSession
    {
        public enum PacketID
        {
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
            Console.WriteLine($"OnConnected Server _ {endPoint}");
        }

        public override void OnDisConnented(EndPoint endPoint)
        {
            Console.WriteLine($"DisConnected {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketID)id) {
                case PacketID.PlayerInfo: {
                    long playerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                    count += 8;
                    Console.WriteLine($"playerId _ {playerId}");
                }
                break;
            }
            
            
            Console.WriteLine($"Receive Size {size} , id : {id}");
        }

        public override void OnSend(int sendData)
        {
            Console.WriteLine("Send 뭐.. 아직은 별로 필요하진 않음");
        }
    }
}
