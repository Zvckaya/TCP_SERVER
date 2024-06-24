
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string genPacket;

        static void Main(string[] args)
        {
            XmlReaderSettings setting = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            using (XmlReader r = XmlReader.Create("PDL.xml", setting))
            {
                r.MoveToContent();
                while (r.Read())
                {
                  if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                        ParsePack(r);

                }

                File.WriteAllText("GenPackets.cs", genPacket);
            }
        }

        public static void ParsePack(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement)
                return;

            if (r.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid packet node");
                return;
            }

            string packetName = r["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }

            ParseMembers(r);
            
        }

        public static void ParseMembers(XmlReader r) { 
            string packName = r["name"];

            int depth = r.Depth + 1;



            while (r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return;
                }

                string memberType= r.Name.ToLower();
                switch (memberType)
                {
                    case "bool":
                    case "byte":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                    case "string":
                    case "list":
                        break;
                    default:
                        break;


                }

            }
        }

    }

}