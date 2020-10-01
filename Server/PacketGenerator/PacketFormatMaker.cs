using Packet;
using System;
using System.Reflection;

namespace PacketGenerator
{
    public class PacketFormatMaker
    {
        public void AssemblyTest()
        {
            // 긁어 오기 테스트
            foreach (var type in Assembly.GetAssembly(typeof(PacketBase)).GetTypes()) {
                Console.WriteLine(type.Name);
            }
        }

    }
}
