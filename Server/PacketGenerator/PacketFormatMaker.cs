using System;
using System.Reflection;

namespace PacketGenerator
{
    public class PacketFormatMaker
    {
        public void AssemblyTest()
        {
            // Assembly.LoadFrom을 사용해서 불러 올 수 있는지 확인하고
                // 이게 된다면, 솔루션에 있는 프로젝트 경로를 가져와서, 어셈블리를 가져올 수 있도록 해야함
                    // 해당 경로는, 솔루션 추가하면 프로젝트 경로 생기는데 그걸로 가져올 수 있는지 확인해보자

                //var assemble = Assembly.LoadFrom();
            
            var types = AppDomain.CurrentDomain.GetAssemblies();
            //.SelectMany(s => s.GetTypes())
            //.Where(p => string.Equals(p.Namespace, "PacketGenerator"));

            foreach (var t in types)
            {
                Console.WriteLine($"{t}");
                //if (t.Name == "TestPack2")
                //{
                //    Console.WriteLine($"{t}");
                //}

                //var tt = t.Assembly.GetReferencedAssemblies();
                //foreach (var ttt in tt) {
                //    Console.WriteLine($"{t} _ {ttt}");
                //}

                //if (!string.Equals(t.Namespace, "PacketGenerator")) {
                //    continue;
                //}

                //var at = t.GetCustomAttributes();
                //foreach (var tt in at)
                //{
                //    Console.WriteLine($"{t} _ {tt}");
                //}
            }

            //foreach (var t in types) {
            //    var tapStr = string.Empty;
            //    if (t == type) {
            //        continue;
            //    }

            //    //if (t.Name == type.Name) {
            //    //    continue;
            //    //}

            //    //if (t.Name == typeof(PacketHeader).Name) {
            //    //    continue;
            //    //}
            //    // 일단은 하나만 하자
            //    //if(t.Name != typeof(TestPack2).Name) {
            //    //    continue;
            //    //}

            //    var pcw = GetField(t, tapStr);
            //    var str = pcw.GetResult();
            //    result.Append(str);
            //}

            //var ttt = result.ToString();
            //result.Clear();
            //result.AppendFormat(PacketStrFormat.NameSpace, ttt);

            //var resultStr = result.ToString();
            //var curPath = Environment.CurrentDirectory;
            //const string target = "PacketGenerator";

            //var targetIndex = curPath.IndexOf("PacketGenerator", StringComparison.Ordinal);
            //if (targetIndex == -1) {
            //    Console.WriteLine("Error! _ invalid Path");
            //    return;
            //}

            //var path = curPath.Substring(0, targetIndex + target.Length + 1);
            //path += "PacketSerializer";
            //var targetPath = Path.Combine(path, "GenPackets.cs");
            //Console.WriteLine($"path -> {targetPath}");

            //File.WriteAllText(targetPath, resultStr);
        }

        [CanBeNull]
        public PacketClassWriter GetField(Type t, string tapStr)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.Static | BindingFlags.Instance |
                                       BindingFlags.DeclaredOnly;

            var fieldMembers = t.GetFields(flags);
            if (fieldMembers.Length == 0) {
                Console.WriteLine($"-- {t.Name} Field Member Is Zero--");
                return null;
            }

            var PacketClassName = t.Name;
            
            var pcw = new PacketClassWriter();
            pcw.Create(PacketClassName);

            Console.WriteLine($"{tapStr} -- {PacketClassName} Field Type Check --");
            foreach (var fInfo in fieldMembers) {
                var Tap = tapStr + "\t";
                var fType = fInfo.FieldType;
                var fName = fInfo.Name;
                CheckField(fType, fName, Tap, pcw);
            }

            Console.WriteLine($"{tapStr} -- {PacketClassName} Field Check End --");
            pcw.SetResult();
            return pcw;
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
                    pcw.AppendStringMember(fName);
                    break;
                case TypeCode.Object:
                    if (type.IsGenericType) {
                        CheckGenericType(type, fName, TapStr);
                    } else {
                        Console.WriteLine($"{TapStr} [{fName} : {cType}]");
                        var tapStr2 = TapStr + "\t";
                        
                        var innerClassPCW = GetField(type, tapStr2);
                        if (innerClassPCW == null) {
                            return;
                        }

                        pcw.AppendClassMember(innerClassPCW);
                    }
                    break;
            }
        }

        private void CheckGenericType(Type t, string fName, string tapStr)
        {
            var TapStr = tapStr;

            var genType = t.GetGenericTypeDefinition();
            Type[] arguments = t.GetGenericArguments();

            Console.WriteLine($"{TapStr} [{fName} : {genType}]");

            // 여기서 CLASS 일경우, 다시 필드 검사 할 수 있도록 만들어야 함
            var pcw = new PacketClassWriter();
            pcw.Create(fName);
            
            TapStr += "\t";
            foreach (var argue in arguments) {
                CheckField(argue, fName, TapStr, pcw);
            }
            pcw.SetResult();
        }

        // 자 1차 목표는 달성했어
        // 인터페이스를 상속받은 클래스를 받아 오는 것
    }

    // 다음이 문제인데,,
        // List<Class> 같은 애들은 시발 어떻게하지
            // 컬랙션일 때, 어떻게 미리 만들어둘 수 있는 방법 없을까?
}
