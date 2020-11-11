using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    public class PacketClassAttribute : Attribute
    {

    }

    public class PacketPropertyAttribute : Attribute
    {
        private int _index;

        public PacketPropertyAttribute(int index) 
        {
            _index = index;
        }
    }
}
