using System;
using System.Collections.Generic;
using System.Text;

namespace Packet
{
    public enum PacketID
    {
        Dummy,
        PlayerInfo
    }

    public struct PacketHeader 
    {
        public ushort Size;
        public ushort Id;
 
        public void SetHeaderData(ref Span<byte> s, ArraySegment<byte> array) 
        {
            bool success = true;
            int count = 0;
            success &= BitConverter.TryWriteBytes(s.Slice(count, array.Count - count), Size);
            count += sizeof(ushort);
            
            success &= BitConverter.TryWriteBytes(s.Slice(count, array.Count - count), Id);
            count += sizeof(ushort);
        }

        public void GetHeaderData(ArraySegment<byte> array)
        {
            var arr = array.ToArray();

            int count = 0;

            Size = (ushort)BitConverter.ToInt16(new ReadOnlySpan<byte>(arr, array.Offset, array.Count));
            count += sizeof(ushort);

            Id = (ushort)BitConverter.ToInt16(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
        }
    }

    public interface PacketBase
    {
        bool Write(ref Span<byte> s, ref int count, ArraySegment<byte> array);
        void Read(ArraySegment<byte> array);
    }

    public class TestInnerClassTest : PacketBase 
    {
        public class TestInnerClass {
            public int test;
            public long test1;

            public bool Wirte(ref Span<byte> s, ref int count, ArraySegment<byte> array) 
            {
                bool success = true;
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), test);
                count += sizeof(int);

                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), test1);
                count += sizeof(long);

                return success;
            }

            public int Read(ArraySegment<byte> array) 
            {
                /// Header Size
                int count = 0;

                BitConverter.ToInt32(new ReadOnlySpan<byte>(array.Array, array.Offset + count, sizeof(int)));
                count += sizeof(int);

                BitConverter.ToInt32(new ReadOnlySpan<byte>(array.Array, array.Offset + count, sizeof(long)));
                count += sizeof(long);

                return count;
            }
        }

        public TestInnerClass _testInnerClass = new TestInnerClass();

        public bool Write(ref Span<byte> s, ref int count, ArraySegment<byte> array) 
        {
            bool success = true;
            
            success &= _testInnerClass.Wirte(ref s, ref count, array);
            
            return success;
        }

        public void Read(ArraySegment<byte> array) 
        {
            int count = 4;
            count += _testInnerClass.Read(array);

            //이런 형태가 나와야 한다.
        }
    }

    public class TestPack2: PacketBase 
    {
        public string PackTest;
        public int PackTT;
        public Dictionary<string, int> TestDic;
        public int TestId;

        public bool Write(ref Span<byte> s, ref int count, ArraySegment<byte> array) { return true; }
        public void Read(ArraySegment<byte> array) { }
    }

    public class TestPack : PacketBase
    {
        PacketHeader Header = new PacketHeader();
        public long PlayerId;
        public string PlayerName;
        public List<int> TestList;
        public Dictionary<string, int> TestDic;
        public int TestId;
        public ulong TestU;
        public Dictionary<ulong, int> TestDic2;

        public TestPack()
        {
            Header.Id = (ushort)PacketID.PlayerInfo;
        }

        public TestPack(PacketHeader header)
        {
            Header = header;
        }

        public bool Write(ref Span<byte> s, ref int count, ArraySegment<byte> array)
        {
            bool success = true;
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), PlayerId);
            count += sizeof(long);

            ushort strLenth = (ushort)Encoding.Unicode.GetBytes(PlayerName, 0, PlayerName.Length, array.Array, array.Offset + count + sizeof(ushort));
            BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), strLenth);
            count += sizeof(ushort);
            count += strLenth;

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)TestList.Count);
            count += sizeof(ushort);

            foreach (var list in TestList) {
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), list);
                count += sizeof(int);
                if (!success) { 
                    return success;
                }
            }

            Header.Size = (ushort)count;
            Header.SetHeaderData(ref s, array);
            
            return success;
        }

        public void Read(ArraySegment<byte> array)
        {
            int count = 4;

            var arr = array.Array;
            PlayerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(long);

            var strlen = BitConverter.ToInt16(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(ushort);
            PlayerName = Encoding.Unicode.GetString(array.Array, array.Offset + count, strlen);
            count += strlen;

            var testListLenth = BitConverter.ToInt16(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(ushort);

            var list = new List<int>();
            for (int i = 0; i < testListLenth; i++) {
                var target = BitConverter.ToInt32(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
                list.Add(target);
                count += sizeof(int);
            }
            TestList = list;
        }
    }
}
