using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Text;

namespace PacketGenerator
{
    public class PacketStrFormat 
    {
        public static string NameSpace =
@"
using System;
using System.Collections.Generic;
using System.Text;
using Packet;

namespace PacketGenerator 
{{
    {0}
}}";

        public static string ClassStr =
@"
    public class {0}_Serializer 
    {{
        public {0} _{0} = new {0}();
        {1}
    }}
";
        
        public static string SerializeFormatStr =
@"
        public bool Serialize(ref Span<byte> s, ref int count, ArraySegment<byte> array) 
        {{
            bool success = false;
            {0}
            return success;
        }}
";

        public static string DeSerializeFormatStr =
@"
        public void DeSerialize(ArraySegment<byte> array) 
        {{
            int count = 0;
            var arr = array.Array;
            {0}
        }}
";

        public static string WriteStr =
@"
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), _{0}.{1}); 
            count += sizeof({2});
";
        public static string WriteStringStr =
@"
            ushort strLenth = (ushort)Encoding.Unicode.GetBytes(_{0}.{1}, 0, _{0}.{1}.Length, array.Array, array.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), strLenth);
            count += sizeof(ushort);
            count += strLenth;
";

        public static string ReadStr = 
@"
            {0} {1} = BitConverter.To{2}(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof({3});
";

        public static string ReadStringStr =
@"
            ushort {0}_strLenth = (ushort)BitConverter.ToInt16(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(ushort);
            string {0} = Encoding.Unicode.GetString(array.Array, array.Offset + count, {0}_strLenth);
            count += {0}_strLenth;
";
    }

    public class PacketClassWriter
    {
        StringBuilder _sb = new StringBuilder();
        StringBuilder _writeSb = new StringBuilder();
        StringBuilder _readSb = new StringBuilder();
        string _className;

        public void Create(string className)
        {
            _className = className;
            _sb.Clear();
            //_sb.Append(string.Format(PacketStrFormat.ClassStr, clasName));
            
            _writeSb.Clear();
            _readSb.Clear();
        }

        public string End() 
        {
            // 여기서 붙여줘야 한다.
            var result = string.Format(PacketStrFormat.SerializeFormatStr, _writeSb.ToString());
            var result1 = string.Format(PacketStrFormat.DeSerializeFormatStr, _readSb.ToString());

            var resultText = new StringBuilder();

            resultText.AppendFormat(PacketStrFormat.ClassStr, _className, result + "\n" + result1);
            var r = resultText.ToString();
            return r;
        }

        public void AppendMember(string fieldName, TypeCode tc) 
        {
            var typeStr = ConvertConstToStr(tc);
            _writeSb.AppendFormat(PacketStrFormat.WriteStr, _className, fieldName, typeStr);
            _readSb.AppendFormat(PacketStrFormat.ReadStr, typeStr, fieldName, tc.ToString(), typeStr);
        }

        public void AppendStringMember(string fieldName) 
        {
            _writeSb.AppendFormat(PacketStrFormat.WriteStringStr, _className, fieldName);
            _readSb.AppendFormat(PacketStrFormat.ReadStringStr, fieldName);
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
