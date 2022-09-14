using System;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string packetEnums;
        static ushort packetId;
        static string genPackets;

        //패킷 처리 함수 등록 포맷
        static string serverRegister;
        static string clientRegister;

        static string pdlPath = "../PDL.xml";
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                pdlPath = args[0];
                Console.WriteLine($"PDL path {pdlPath}");
                
            }
                
            
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                //주석, 스페이스 바 무시
                IgnoreComments = true,
                IgnoreWhitespace = true,

            };

            
            
            //PDL.xml 파일 넘겨 받기
            using(XmlReader r = XmlReader.Create(pdlPath, settings))
            {
                r.MoveToContent();
                while (r.Read())
                {
                    //패킷 파싱 & </packet> x
                    if(r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                        ParsePacket(r);
                    
                }
            }
            string fileText = string.Format(PacketFormat.FileFormat, packetEnums, genPackets);
            File.WriteAllText("GenPackets.cs", fileText);

           //클라->서버
           //패킷을 어떻게 처리할지
            string serverManagerText = string.Format(PacketFormat.ManagerFormat, serverRegister);
            File.WriteAllText("ServerPacketManager.cs", serverManagerText);

            string clientManagerText = string.Format(PacketFormat.ManagerFormat, clientRegister);
            File.WriteAllText("ClientPacketManager.cs", clientManagerText);
        }
        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement)
                return;

            if (r.Name.ToLower() != "packet")
                return;

            string packetName = r["name"];
            if (string.IsNullOrEmpty(packetName))
                return;

            Tuple<string, string, string> t = ParseMembers(r);
            genPackets += String.Format(PacketFormat.packetFormat, packetName, t.Item1, t.Item2, t.Item3);
            packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetId) + "\t\t";

            //클라->서버(처리)
            if (packetName.StartsWith("C_")||packetName.StartsWith("c_"))
                serverRegister += string.Format(PacketFormat.ManagerRegisterFormat, packetName);
            //서버->클라(처리)
            else
                clientRegister += string.Format(PacketFormat.ManagerRegisterFormat, packetName);

        }

        public static Tuple<string,string,string> ParseMembers(XmlReader r)
        {
            string packetName = r["name"];
            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            //직계 자손만 파싱
            int depth = r.Depth + 1;
            while (r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    return null;
                }
                if (string.IsNullOrEmpty(memberCode) == false)
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
                        readCode += string.Format(PacketFormat.readFormat, memberName,ToMemberType(memberType) , memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);

                        break;
                    case "list":
                        {

                        Tuple<string, string, string> t = ParseList(r);
                        memberCode += t.Item1;
                        readCode += t.Item2;
                        writeCode += t.Item3;

                        }
                        
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
            string listName = r["name"];
            if (string.IsNullOrEmpty(listName))
                return null;

            Tuple<string, string, string> t = ParseMembers(r);

            //구조체 멤버 read, write
            string memberCode = string.Format(PacketFormat.memberListFormat, 
                FirstCharToUpper(listName), 
                FirstCharToLower(listName), 
                t.Item1,
                t.Item2, 
                t.Item3);
            //리스트내의 구조체 read, write
            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            string writeCode = string.Format(PacketFormat.writeListFormat,
               FirstCharToUpper(listName),
               FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }
        public static string FirstCharToUpper(string listName)
        {
            if (string.IsNullOrEmpty(listName))
                return "";
            return listName[0].ToString().ToUpper() + listName.Substring(1);
        }
        public static string FirstCharToLower(string listName)
        {
            if (string.IsNullOrEmpty(listName))
                return "";
            return listName[0].ToString().ToLower() + listName.Substring(1);
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
