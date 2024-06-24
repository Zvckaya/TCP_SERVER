
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

            Tuple<string,string,string> t = ParseMembers(r);
            genPacket += string.Format(PacketFormat.packetFomat, packetName, t.Item1, t.Item2,t.Item3);
            
        }

        public static Tuple<string,string,string> ParseMembers(XmlReader r) { 
            string packetName = r["name"];

            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = r.Depth + 1;



            while (r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                if(string.IsNullOrEmpty(memberCode)==false)
                {
                    memberCode += Environment.NewLine;
                }
                if (string.IsNullOrEmpty(readCode) == false)
                {
                    memberCode += Environment.NewLine;
                }
                if (string.IsNullOrEmpty(writeCode) == false)
                {
                    memberCode += Environment.NewLine;
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
                        memberCode += string.Format(PacketFormat.memberFormat,memberType,memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName, memberType);
                        break;
                    case "list":
                        break;
                    default:
                        break;


                }

            }

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                    return "ToBoolean";
                
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";

            }
        }

    }

}