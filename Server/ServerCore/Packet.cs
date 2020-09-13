﻿using System;
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

    public class TestPack : PacketBase
    {
        PacketHeader Header = new PacketHeader();
        public long PlayerId;
        public string PlayerName;
        public List<int> TestList;

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
            ////이게 된다고 ? 왜?
            //var strLenth = (ushort)Encoding.Unicode.GetBytes(PlayerName, 0, PlayerName.Length, s.ToArray(), count + sizeof(ushort));
            //BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), strLenth);
            //count += sizeof(ushort);
            //count += strLenth;

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