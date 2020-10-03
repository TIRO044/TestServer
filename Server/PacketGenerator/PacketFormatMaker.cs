using Microsoft.VisualBasic.CompilerServices;
using Packet;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PacketGenerator
{
    public class PacketFormatMaker
    {
        public void AssemblyTest()
        {
            // 긁어 오기 
            var type = typeof(PacketBase);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                                               .SelectMany(s => s.GetTypes())
                                               .Where(p => type.IsAssignableFrom(p));

            var result = new StringBuilder();

            foreach (var t in types) {
                var tapStr = string.Empty;
                if (t.GetType() == type) {
                    continue;
                }

                var str = GetField(t, tapStr);
                result.Append(str);
                var tt = result.ToString();
            }

            var ttt = result.ToString();
            result.Clear();
            result.AppendFormat(PacketStrFormat.NameSpace, ttt);

            var resultStr = result.ToString();
            var curPath = Environment.CurrentDirectory;
            var target = "PacketGenerator";

            var targetIndex = curPath.IndexOf("PacketGenerator");
            if (targetIndex == -1) {
                Console.WriteLine("Error! _ invaild Path");
                return;
            }

            var path = curPath.Substring(0, targetIndex + target.Length + 1);
            path += "PacketSerializer";
            var targetPath = Path.Combine(path, "GenPackets.cs");
            Console.WriteLine($"path -> {targetPath}");

            File.WriteAllText(targetPath, resultStr);
        }

        public string GetField(Type t, string tapStr)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;

            var fieldMembers = t.GetFields(flags);
            if (fieldMembers.Length == 0) {
                Console.WriteLine($"-- {t.Name} Field Member Is Zero--");
                return string.Empty;
            }

            var PacketClassName = t.Name;
            
            PacketClassWriter pcw = new PacketClassWriter();
            pcw.Create(PacketClassName);

            Console.WriteLine($"{tapStr} -- {PacketClassName} Field Type Check --");
            foreach (var fInfo in fieldMembers) {
                var Tap = tapStr + "\t";
                var fType = fInfo.FieldType;
                var fName = fInfo.Name;
                CheckField(fType, fName, Tap, pcw);
            }

            Console.WriteLine($"{tapStr} -- {PacketClassName} Field Check End --");
            return pcw.End();
        }

        private void CheckField(Type type, string fName, string tapStr, PacketClassWriter pcw)
        {
            var TapStr = tapStr;

            var cType = Type.GetTypeCode(type);
            switch (cType) {
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
                    Console.WriteLine($"{TapStr} [{fName} : {cType}]");
                    pcw.AppendMember(fName, cType);
                    break;
                case TypeCode.Boolean:
                    Console.WriteLine($"{TapStr} [{fName} : {cType}]");
                    pcw.AppendMember(fName, cType);
                    break;
                case TypeCode.String:
                    // 특별한 처리 필요함
                    Console.WriteLine($"{TapStr} [{fName} : {cType}]");
                    //pcw.AppendMember(fName, cType);
                    break;
                case TypeCode.Object:
                    if (type.IsGenericType) {
                        CheckGenericType(type, fName, TapStr);
                    } else {
                        Console.WriteLine($"{TapStr} [{fName} : {cType}]");
                        var tapStr2 = TapStr + "\t";
                        GetField(type, tapStr2);
                    }
                
                    break;
                default:
                    break;
            }
        }

        private void CheckGenericType(Type t, string fName, string tapStr)
        {
            var TapStr = tapStr;

            var genType = t.GetGenericTypeDefinition();
            Type[] arguments = t.GetGenericArguments();

            Console.WriteLine($"{TapStr} [{fName} : {genType}]");

            TapStr += "\t";

            PacketClassWriter pcw = new PacketClassWriter();
            pcw.Create(fName);
            foreach (Type argu in arguments) {
                CheckField(argu, fName, TapStr, pcw);
            }
            pcw.End();
        }

        // 자 1차 목표는 달성했어
        // 인터페이스를 상속받은 클래스를 받아 오는 것
    }

    //2차 목표 시작
    //일단, 시리얼 라이저 포멧을 구상해보자
    public class PacketSerializer {

        public class TestPacket1_Serializer 
        {
            public bool Serialize(ref Span<byte> s, ref int count, ArraySegment<byte> array)
            {
                return true;
            }

            public void DeSerialize(ArraySegment<byte> array)
            {

            }
        }
    }
}
