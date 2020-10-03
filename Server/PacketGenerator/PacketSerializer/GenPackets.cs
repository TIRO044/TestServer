
using System;
using System.Collections.Generic;
using System.Text;
using Packet;

namespace PacketGenerator 
{
    
    public class TestPack2_Serializer 
    {
        public TestPack2 _TestPack2 = new TestPack2();
        
        public bool Serialize(ref Span<byte> s, ref int count, ArraySegment<byte> array) 
        {
            bool success = false;
            
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), _TestPack2.PackTT); 
            count += sizeof(int);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), _TestPack2.TestId); 
            count += sizeof(int);

            return success;
        }


        public void DeSerialize(ArraySegment<byte> array) 
        {
            int count = 0;
            var arr = array;
            
            int PackTT = BitConverter.ToInt32(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(int);

            int TestId = BitConverter.ToInt32(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(int);

        }

    }

    public class TestPack_Serializer 
    {
        public TestPack _TestPack = new TestPack();
        
        public bool Serialize(ref Span<byte> s, ref int count, ArraySegment<byte> array) 
        {
            bool success = false;
            
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), _TestPack.PlayerId); 
            count += sizeof(long);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), _TestPack.TestId); 
            count += sizeof(int);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), _TestPack.TestU); 
            count += sizeof(ulong);

            return success;
        }


        public void DeSerialize(ArraySegment<byte> array) 
        {
            int count = 0;
            var arr = array;
            
            long PlayerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(long);

            int TestId = BitConverter.ToInt32(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(int);

            ulong TestU = BitConverter.ToUInt64(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(ulong);

        }

    }

}