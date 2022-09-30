using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    
    //{0}패킷 이름/ 패킷 번호
    //{1}packetFormat 들
    class PacketFormat
    {
        //{0}패킷 등록
        public static string managerFormat =
@"
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
    {{
        //<id , req패킷을 어떻게 처리 할지>
        static Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
        static Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

        static PacketManager _instance = new PacketManager();

        public static PacketManager Instance
        {{
            get
            {{
                return _instance;
            }}
        }}
        PacketManager()
        {{
            Register();
        }}

        public void Register()
        {{
            {0}
        }}
        public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
        {{
            ushort count = 0;
            count += 2;
            ushort packetid = BitConverter.ToUInt16(buffer.Array, count);
            count += 2;

            Action<PacketSession, ArraySegment<byte>> action;
            if(_onRecv.TryGetValue(packetid ,out action))
            {{
                action.Invoke(session, buffer);
            }}

        }}
        public static void MakePacket<T>(PacketSession session,ArraySegment<byte> buffer)where T :IPacket,new()
        {{
            T p = new T();
            p.Read(buffer);

            Action<PacketSession, IPacket> action;
            if(_handler.TryGetValue((ushort)p.Protocol, out action))
            {{
                action.Invoke(session,p);
            }}

        }}


      
    }}
";
        //{0}패킷 이름
        public static string managerRegisterFormat =
@"
        _onRecv.Add((ushort)PacketId.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketId.{0},PacketHandler.{0}Handler);
";
        public static string fileFormat =
@"
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

enum PacketId
{{
    {0}
}}

interface IPacket
{{
	ushort Protocol {{ get;}}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}}

    {1}
";
        //{0}패킷 이름
        //{1}패킷 번호
        public static string packetEnumFormat =
@"
{0} = {1},
";
        //{0}패킷 이름
        //{1}멤버 변수
        //{2}멤버 read
        //{3}멤버 write
        public static string packetFormat =
@"
 public class {0} :IPacket
{{
    {1}
    
    public ushort Protocol {{ get{{ return (ushort)PacketId.{0} ; }} }}

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);


        count += 2;//size
        count += 2;//packetid

        {2}

    }}

    public ArraySegment<byte> Write()
    {{

        //보낸다!
        ushort count = 0;
        bool success = true;
        //WriteBuffer
        ArraySegment<byte> segment = SendBuffHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

               
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketId.{0});
        count += sizeof(ushort);

        {3}

        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;

        return SendBuffHelper.Close(count);
    }}
}}
";
        //{0} 변수 타입
        //{1} 변수 이름
        public static string memberFormat =
@"
    public {0} {1};
";
        //{0}구조체 이름[대문자]
        //{1}구조체 이름[소문자]

        //{2}멤버 변수
        //{3}멤버 read
        //{4}멤버 write
        public static string memeberListFormat =
@"
public List<{0}> {1}s = new List<{0}>();

public struct {0}
{{
    {2}

    public void Read(ReadOnlySpan<byte> s, ref ushort count)
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
        //{0}변수 이름
        //{1}ToInt~
        //{2}변수 타입
        public static string readFormat =
@"
this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});
";
        //{0}변수 이름
        public static string readStringFormat =
@"
ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);

this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len;
";
        //{0}구조체 이름[대문자]
        //{1}구조체 이름[소문자]
        public static string readListFormat =
@"
ushort {1}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);

{1}s.Clear();

for (int i = 0; i < {1}Len; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}
";
        //{0}변수 이름
        //{1}변수 타입
        public static string readByteFormat =
@"
this.{0} = segment.Array[segment.Offset + count];
count += sizeof({1});
";
        //{0}변수 이름
        //{1}변수 타입
        public static string writeFormat =
@"
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0});
count += sizeof({1});
";
        //{0}변수 이름
        public static string writeStringFormat =
@"
ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);

count += sizeof(ushort);
count += {0}Len;
";
        //{0}구조체 이름[대문자]
        //{1}구조체 이름[소문자]
        public static string writeListFormat =
@"
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort){1}s.Count);
count += sizeof(ushort);

foreach ({0} {1} in {1}s)
        {1}.Write(s, ref count);
";
        //{0}변수 이름
        //{1}변수 타입
        public static string writeByteFormat =
@"
segment.Array[segment.Offset + count] = {0};
count += sizeof({1});
";
    }
}
