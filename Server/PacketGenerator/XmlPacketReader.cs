using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace PacketGenerator
{
    public class XmlPacketReader
    {
        public void SettingManager()
        {
            XmlReaderSettings setting = new XmlReaderSettings() {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            using XmlReader r = XmlReader.Create("PDL.xml", setting);
            { 
                r.MoveToContent();
                while (r.Read()) {
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element) { 
                        //PacketParse(r);
                    }
                }

                //File.WriteAllText("GenPackets.cs", genPackets);
            }
        }
    }
}
