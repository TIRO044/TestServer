using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PacketGenerator
{
    public class PacketFormatMaker
    {
        static string _tab;

        static string Tab {
            get {
                _tab += "\t";
                return _tab;
            }   
        }

        public void AssemblyTest()
        {
            // 긁어 오기 
            var type = typeof(PacketBase);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach (var t in types) {
                if (t.GetType() == type) {
                    continue;
                }

                GetField(t);
            }
        }

        public void GetField(Type t)
        {
            Console.WriteLine($"-- {t.Name} start --");


            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;

            var fieldMembers = t.GetFields(flags);

            foreach (var fInfo in fieldMembers) {
                var fType = fInfo.FieldType;
                CheckField(fType);
            }

            Console.WriteLine($"-- {t.Name} end --");
        }

        private void CheckField(Type type)
        {
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
                    Console.WriteLine($"number contant {cType}");
                    break;
                case TypeCode.String:
                    Console.WriteLine($"string contant {cType}");
                    break;
                case TypeCode.Boolean:
                    Console.WriteLine($"boolean contant {cType}");
                    break;
                case TypeCode.Object:
                    if (type.IsGenericType) {
                        CheckGenericType(type);
                    } else {
                        Console.WriteLine($"is Object {cType}");
                    }
                    break;
                default:
                    break;
            }
        }

        private void CheckGenericType(Type t)
        {
            var genType = t.GetGenericTypeDefinition();
            Type[] arguments = t.GetGenericArguments();

            if (genType == typeof(List<>)) {
                Console.WriteLine($"\t is List");
            } else if (genType == typeof(Dictionary<,>)) {
                Console.WriteLine($"\t is Dic");
            }

            foreach (Type argu in arguments) {
                CheckField(argu);
            }
        }

        // 자 1차 목표는 달성했어
        // 인터페이스를 상속받은 클래스를 받아 오는 것
    }
}

// 최종 목표는 무었이냐
// 패킷 클래스를 만들면, 패킷 안에 있는 필드들을 검사해서 패킷 시리얼라이즈, 디시리얼라이즈를 만들어 주는 것을 자동화 하는 것

// 그렇게 하기 위해선 우선 되어야 하는 것은 ?
// 1차 목표 
//패킷 인터페이스를 상속 받는 클래스 들을 모두 가져와야 한다.
// 2차 목표
// 필드들을 모두 검사한다.
// 필드들의 타입을 검사한다.
// 타입에 따른 시리얼 라이즈 코드를 만들어 준다.
// 3차 목표
// 자동화 코드를 만들어준다. 상세 목표는 아직