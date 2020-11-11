using ServerCore;
using System;
using System.Net;
using Packet;

namespace Client
{
    class ServerSession : PacketSession
    {
        PacketHeader _recievePack = new PacketHeader();

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
            _recievePack.GetHeaderData(buffer);

            switch ((PacketID)_recievePack.Id) {
                case PacketID.PlayerInfo: {
                    var tp = new TestPack(_recievePack);
                    tp.Read(buffer);
                    //long playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count));
                    //count += 8;
                    Console.WriteLine($"playerId _ {tp.PlayerId}");
                    foreach (var t in tp.TestList) {
                            Console.WriteLine($"t _ {t}");    
                    }
                }
                break;
            }
            
            
            Console.WriteLine($"Receive Size {_recievePack.Size} , id : {_recievePack.Id}");
        }

        public override void OnSend(int sendData)
        {
            Console.WriteLine("Send 뭐.. 아직은 별로 필요하진 않음");
        }
    }
}
