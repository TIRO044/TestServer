using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PacketGenerator
{
    public class PacketStrFormat 
    {
        public static string ClassStr = 
            @"public class {0}_Serializer 
              {{
                    {1}
              }}";
        
        public static string SerializeFormatStr =
           @"public bool Serialize(ref Span<byte> s, ref int count, ArraySegment<byte> array) 
             {{
                    bool success = false;
                    int count = 0;
                    {0}
             }}";

        public static string DeSerializeFormatStr = 
            @"public void DeSerialize(ArraySegment<byte> array) 
              {{
                    int count = 0;
                    {0}
              }}";

        public static string WriteStr =
            @"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}); 
              count += sizeof({1});";

        public static string ReadStr = 
            @"BitConverter.To{0}(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
              count += sizeof{1}";

        public static string Tab = "\t";
    }

    public class PacketClassWriter
    {
        StringBuilder _sb = new StringBuilder();
        StringBuilder _writeSb = new StringBuilder();
        StringBuilder _readSb = new StringBuilder();

        public void Create(string clasName)
        {
            _sb.Clear();
            _sb.Append(string.Format(PacketStrFormat.ClassStr, clasName));
            
            _writeSb.Clear();
            _readSb.Clear();
        }

        public void End() 
        {
            // 여기서 붙여줘야 한다.
        }

        public void AppendMember(string fieldName, TypeCode tc) 
        {
            ConvertConstToStr(tc);
        }

        private void ConvertConstToStr(TypeCode tc)
        {
            switch (tc) {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    break;
                case TypeCode.String:
                    break;
                case TypeCode.Boolean:
                    break;
                case TypeCode.Object:
                    break;
                default:
                    break;
            }
        }
    }
}
