using System;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static ushort packetid = 0;
        static string enums = "";
        static string fileTexts = "";
        static string genPackets = "";
        static string serverManagerRegister = "";
        static string clientManagerRegister = "";

        static void Main(string[] args)
        {
            string pdlPath = "../PDL.xml";
            if (args.Length >= 1)
                pdlPath = args[0];

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,//주석무시
                IgnoreWhitespace = true,

            };
            
            using(XmlReader r = XmlReader.Create(pdlPath, settings))
            {
                r.MoveToContent();

                while (r.Read())
                {
                    //여기부터 패킷
                    if(r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                        ParsePacket(r);
                    
                }
                fileTexts = string.Format(PacketFormat.fileFormat, enums, genPackets);
                File.WriteAllText("GenPackets.cs", fileTexts);

                string serverManagerText = string.Format(PacketFormat.managerFormat, serverManagerRegister);
                File.WriteAllText("ServerPacketManager.cs", serverManagerText);

                string clientManagerText = string.Format(PacketFormat.managerFormat, clientManagerRegister);
                File.WriteAllText("ClientPacketManager.cs", clientManagerText);
            }

        }
        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement)
                return;

            if (r.Name.ToLower() != "packet")
                return;

            //NULL체크
            string packetName = r["name"];
            if (string.IsNullOrEmpty(packetName))
                return;

            //패킷 형식 완성
            Tuple<string,string,string> t = ParseMembers(r);
            genPackets += string.Format(PacketFormat.packetFormat, packetName,t.Item1, t.Item2, t.Item3);
            enums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetid);
            //클라에서 온 패킷/서버에서 온 패킷 구분
            if (packetName.StartsWith('C') || packetName.StartsWith('c'))
                serverManagerRegister += string.Format(PacketFormat.managerRegisterFormat, packetName);
            else
                clientManagerRegister += string.Format(PacketFormat.managerRegisterFormat, packetName);


        }
        public static Tuple<string,string,string> ParseMembers(XmlReader r)
        {
            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = r.Depth + 1;

            //멤버 파싱
            while (r.Read())
            {
                if (r.Depth != depth)
                    break;
                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                    return null;

                //null이 아니면 한줄 띄우기
                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(readCode) == false)
                    memberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(writeCode) == false)
                    memberCode += Environment.NewLine;
                
                string memberType = r.Name.ToLower();

                switch (memberType)
                {
                    case "byte":
                    case "sbyte":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                        break;
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> t = ParseList(r);
                        memberCode += t.Item1;
                        readCode += t.Item2;
                        writeCode += t.Item3;
                        break;
                    default:
                        break;

                }
            }
            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }
        public static Tuple<string,string,string> ParseList(XmlReader r)
        {
            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            string structName = r["name"];
            if (string.IsNullOrEmpty(structName))
                return null;
            //클래스 안의 작은 클래스 (재귀적)
            Tuple<string, string, string> t = ParseMembers(r);
            memberCode = string.Format(PacketFormat.memeberListFormat,
                FirstCharToUpper(structName),
                FirstCharToLower(structName),
                t.Item1,
                t.Item2,
                t.Item3);
            readCode = string.Format(PacketFormat.readListFormat, FirstCharToUpper(structName), FirstCharToLower(structName));
            writeCode = string.Format(PacketFormat.writeListFormat, FirstCharToUpper(structName), FirstCharToLower(structName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);

        }
        //기본 자료형 -> 바이트 배열
        //문자열 -> 바이트 배열?
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
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                case "long":
                    return "ToInt64";
                case "ulong":
                    return "ToUInt64";
                default:
                    break;
            }
            return null;
        }
        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            return input[0].ToString().ToUpper() + input.Substring(1);

        }
        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}
