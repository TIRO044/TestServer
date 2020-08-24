using ServerCore;
using System;
using System.Net;

namespace Client
{
    class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint) 
        {
            Console.WriteLine($"DisConnected {endPoint}");
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
