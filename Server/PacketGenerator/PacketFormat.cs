using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    //자동화 템플릿
    class PacketFormat
    {
        //{0} 딕셔너리에 등록
        public static string ManagerFormat =
@"

using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{{
    //싱글톤 패턴(클래스명으로 접근)
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance {{ 
        get 
        {{
            if (_instance == null)
                _instance = new PacketManager();
            return _instance; 
        }}
    }}
    //프로토콜 아이디 -> 알맞은 함수 실행
    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    //패킷 핸들러 연결
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    //딕셔너리에 한번만 등록
    public void Register()
    {{
        {0}
    }}
    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buff)
    {{
          
        ushort count = 0;
        //패킷 아이디 -> 함수<패킷 타입> 실행
        ushort size = BitConverter.ToUInt16(buff.Array, buff.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buff.Array, buff.Offset + 2);
        count += 2;
        //함수 탐색
        Action<PacketSession, ArraySegment<byte>> _action = null;
        if(_onRecv.TryGetValue(id , out _action))
        {{
            //함수 실행
            _action.Invoke(session, buff);
        }}
            

    }}
    //패킷타입 핸들러 실행
    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T: IPacket , new()
    {{
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;

        if (_handler.TryGetValue(pkt.Protocol, out action))
            action.Invoke(session,pkt);
            
    }}
}}
";
        //{0} 패킷 이름
        public static string ManagerRegisterFormat =
@"
        _onRecv.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);
";
        //{0} 패킷 이름 / 번호
        //{1} 패킷 목록

        public static string FileFormat =
@"
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

interface IPacket
{{
	public ushort Protocol {{ get; }}
	public void Read(ArraySegment<byte> segment);
	public ArraySegment<byte> Write();
}}

public enum PacketID
{{
    {0}

}}

{1}

";
        //{0} 패킷 이름
        //{1} 패킷 번호
        public static string packetEnumFormat =
@"
{0} = {1},
";
        //{0} 패킷 이름
        //{1} 멤버 변수들
        //{2} 멤버 변수 read
        //{3} 멤버 변수 write
        public static string packetFormat =
@"
public class {0} : IPacket
{{

    {1}

    public ushort Protocol {{get {{ return (ushort)PacketID.{0}; }} }} 

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);//size
        count += sizeof(ushort);//패킷 타입

        {2}
    }}

    public ArraySegment<byte> Write()
    {{
            
        ArraySegment<byte> segment = SendBuffHelper.Open(1024);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

           
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.{0});
        count += sizeof(ushort);

        {3}

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;

        return SendBuffHelper.Close(count);

           
    }}
}}
";
        //{0}멤버 형식
        //{1}멤버 이름
        public static string memberFormat =
@"
public {0} {1};
";
        //{0}변수 이름[대문자]
        //{1}변수 이름[소문자]
        //{2}멤버 변수들
        //{3}멤버 read
        //{4}멤버 write
        public static string memberListFormat =
@"
public List<{0}> {1}s = new List<{0}>();

public struct {0}
{{
    {2}

    public void Read(Span<byte> s, ref ushort count)
    {{
        {3}


    }}
    public bool Write(Span<byte> s, ref ushort count)
    {{
        bool success = true;

        {4}

        return success;
    }}
         
}}
";

        //{0}멤버 이름
        //{1}ToUInt멤버 형식
        //{2}멤버 형식
        public static string readFormat =
@"
{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});
";
        //{0} 변수 이름
        //{1} 변수 타입
        public static string readByteFormat =
@"
{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});
";
        //{0}멤버 이름
       
        public static string readStringFormat =
@"
ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len;
";
        //{0}변수 이름[대문자]
        //{1}변수 이름[소문자]

        public static string readListFormat =
@"
 ushort {1}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
{1}s.Clear();
for(int i = 0; i< {1}Len; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}
";
        //{0}변수 이름
        //{1}변수 타입
        public static string writeFormat =
@"
success &= BitConverter.TryWriteBytes(s.Slice(count , s.Length - count), {0});
count += sizeof({1});
";
        //{0}변수 이름
        //{1}변수 타입
        public static string writeByteFormat =
@"
segment.Array[segment.Offset + count] = ({1})this.{0};
count += sizeof({1});
";
        //{0}변수 이름
        
        public static string writeStringFormat =
@"
ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, {0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, segment.Offset + count), {0}Len);
count += sizeof(ushort);
count += {0}Len;
";
        //{0}변수 이름[대문자]
        //{1}변수 이름[소문자]
        public static string writeListFormat =
@"
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort){1}s.Count);
count += sizeof(ushort);

foreach({0} {1} in {1}s)
{{
    success &= {1}.Write(s, ref count);
}}
";
    }
    

}
