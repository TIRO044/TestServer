using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Schema;

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
            //_sb.Append(string.Format(PacketStrFormat.ClassStr, clasName));
            
            _writeSb.Clear();
            _readSb.Clear();
        }

        public void End() 
        {
            // 여기서 붙여줘야 한다.
        }

        public void AppendMember(string fieldName, TypeCode tc) 
        {
            var str = ConvertConstToStr(tc);
            var testStr = string.Format(PacketStrFormat.WriteStr, fieldName, str);

            Console.WriteLine($"AppendMember Test --- >>> {testStr}");
            //ConvertConstToStr(tc);
        }

        private string ConvertConstToStr(TypeCode tc)
        {
            string returnValue = string.Empty;

            switch (tc) {
                case TypeCode.Byte:
                    returnValue = "byte";
                    break;
                case TypeCode.SByte:
                    returnValue = "sbyte";
                    break;
                case TypeCode.UInt16:
                    returnValue = "ushort";
                    break;
                case TypeCode.UInt32:
                    returnValue = "uint";
                    break;
                case TypeCode.UInt64:
                    returnValue = "ulong";
                    break;
                case TypeCode.Int16:
                    returnValue = "short";
                    break;
                case TypeCode.Int32:
                    returnValue = "int";
                    break;
                case TypeCode.Int64:
                    returnValue = "long";
                    break;
                case TypeCode.Decimal:
                    returnValue = "decimal";
                    break;
                case TypeCode.Double:
                    returnValue = "double";
                    break;
                case TypeCode.Single:
                    returnValue = "Single";
                    break;
                case TypeCode.Boolean:
                    returnValue = "bool";
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.String:
                    returnValue = "string";
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}
