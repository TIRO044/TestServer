
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
            
            ushort strLenth = (ushort)Encoding.Unicode.GetBytes(_TestPack2.PackTest, 0, _TestPack2.PackTest.Length, array.Array, array.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), strLenth);
            count += sizeof(ushort);
            count += strLenth;

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), _TestPack2.PackTT); 
            count += sizeof(int);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), _TestPack2.TestId); 
            count += sizeof(int);

            return success;
        }


        public void DeSerialize(ArraySegment<byte> array) 
        {
            int count = 0;
            var arr = array.Array;
            
            ushort PackTest_strLenth = (ushort)BitConverter.ToInt16(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(ushort);
            string PackTest = Encoding.Unicode.GetString(array.Array, array.Offset + count, PackTest_strLenth);
            count += PackTest_strLenth;

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

            ushort strLenth = (ushort)Encoding.Unicode.GetBytes(_TestPack.PlayerName, 0, _TestPack.PlayerName.Length, array.Array, array.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), strLenth);
            count += sizeof(ushort);
            count += strLenth;

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), _TestPack.TestId); 
            count += sizeof(int);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), _TestPack.TestU); 
            count += sizeof(ulong);

            return success;
        }


        public void DeSerialize(ArraySegment<byte> array) 
        {
            int count = 0;
            var arr = array.Array;
            
            long PlayerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(long);

            ushort PlayerName_strLenth = (ushort)BitConverter.ToInt16(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(ushort);
            string PlayerName = Encoding.Unicode.GetString(array.Array, array.Offset + count, PlayerName_strLenth);
            count += PlayerName_strLenth;

            int TestId = BitConverter.ToInt32(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(int);

            ulong TestU = BitConverter.ToUInt64(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(ulong);

        }

    }

}