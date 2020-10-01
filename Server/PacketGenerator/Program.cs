using System;

namespace PacketGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var t = new PacketFormatMaker();
            t.AssemblyTest();

            while (true) { }
        }
    }
}
