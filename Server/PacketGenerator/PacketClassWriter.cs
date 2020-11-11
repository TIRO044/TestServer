using System;
using System.Collections.Generic;
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
        /// <summary>
        /// 0. ClassName
        /// 1. Method
        /// 2. InnerClass
        /// </summary>
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
        public static string ReadStringStr =
@"
            ushort {0}_strLength = (ushort)BitConverter.ToInt16(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof(ushort);
            string {0} = Encoding.Unicode.GetString(array.Array, array.Offset + count, {0}_strLength);
            count += {0}_strLength;
";
        public static string WriteStringStr =
@"
            ushort strLength = (ushort)Encoding.Unicode.GetBytes(_{0}.{1}, 0, _{0}.{1}.Length, array.Array, array.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), strLength);
            count += sizeof(ushort);
            count += strLength;
";

        public static string ReadStr = 
@"
            {0} {1} = BitConverter.To{2}(new ReadOnlySpan<byte>(arr, array.Offset + count, array.Count - count));
            count += sizeof({3});
";

        public static string WriteInnerClass =
@"
            success &= _{0}.Serialize(ref s, ref count, array);
";

        public static string ReadInnerClass =
@"
            {0} _{0} = new {0}();
            _{0}.DeSerialize(array);
";

    }

    public class PacketClassWriter
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private readonly StringBuilder _writeSb = new StringBuilder();
        private readonly StringBuilder _readSb = new StringBuilder();
        private readonly StringBuilder _resultSb = new StringBuilder();

        public List<string> InnerClassStr { get; } = new List<string>();

        //List<PacketClassWriter> _innerClassPCW = new List<PacketClassWriter>();

        public string ClassName { get; private set; }

        public void Create(string className)
        {
            ClassName = className;
            _sb.Clear();
            _writeSb.Clear();
            _readSb.Clear();
            _resultSb.Clear();
            //_innerClassPCW.Clear();
        }

        public void SetResult() 
        {
            // 여기서 붙여줘야 한다.
            var result = string.Format(PacketStrFormat.SerializeFormatStr, _writeSb);
            var result1 = string.Format(PacketStrFormat.DeSerializeFormatStr, _readSb);

            _resultSb.Clear();
            _resultSb.AppendFormat(PacketStrFormat.ClassStr, ClassName, result + "\n" + result1);
        }

        public string GetResult() 
        {
            return _resultSb.ToString();
        }

        public void AppendMember(string fieldName, TypeCode tc) 
        {
            var typeStr = ConvertConstToStr(tc);
            _writeSb.AppendFormat(PacketStrFormat.WriteStr, ClassName, fieldName, typeStr);
            _readSb.AppendFormat(PacketStrFormat.ReadStr, typeStr, fieldName, tc.ToString(), typeStr);
        }

        public void AppendStringMember(string fieldName) 
        {
            _writeSb.AppendFormat(PacketStrFormat.WriteStringStr, ClassName, fieldName);
            _readSb.AppendFormat(PacketStrFormat.ReadStringStr, fieldName);
        }

        public void AppendClassMember(PacketClassWriter innerPCW) 
        {
            var innerClassName = innerPCW.ClassName;
            _writeSb.AppendFormat(PacketStrFormat.WriteInnerClass, innerClassName);
            _writeSb.AppendFormat(PacketStrFormat.ReadInnerClass, innerClassName);

            //var t = innerPCW.ClassName;
            var tt = innerPCW.GetResult();
            InnerClassStr.Add(tt);
            //_innerClassPCW.Add(innerPCW);
        }

        private string ConvertConstToStr(TypeCode tc)
        {
            var returnValue = string.Empty;

            switch (tc)
            {
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
                case TypeCode.Empty:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.DateTime:
                    break;
            }

            return returnValue;
        }
    }
}
