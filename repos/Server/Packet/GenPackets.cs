
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

enum PacketId
{
    
c_chat = 1,

s_chat = 2,

}

interface IPacket
{
	ushort Protocol { get;}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

    
 public class c_chat :IPacket
{
    
	    public string chat;
	
    
    public ushort Protocol { get{ return (ushort)PacketId.c_chat ; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);


        count += 2;//size
        count += 2;//packetid

        
		ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
		count += chatLen;
		

    }

    public ArraySegment<byte> Write()
    {

        //보낸다!
        ushort count = 0;
        bool success = true;
        //WriteBuffer
        ArraySegment<byte> segment = SendBuffHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

               
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketId.c_chat);
        count += sizeof(ushort);

        
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);
		
		count += sizeof(ushort);
		count += chatLen;
		

        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;

        return SendBuffHelper.Close(count);
    }
}

 public class s_chat :IPacket
{
    
	    public int playerid;
	
	
	
	
	    public string chat;
	
    
    public ushort Protocol { get{ return (ushort)PacketId.s_chat ; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);


        count += 2;//size
        count += 2;//packetid

        
		this.playerid = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
		count += chatLen;
		

    }

    public ArraySegment<byte> Write()
    {

        //보낸다!
        ushort count = 0;
        bool success = true;
        //WriteBuffer
        ArraySegment<byte> segment = SendBuffHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

               
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketId.s_chat);
        count += sizeof(ushort);

        
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerid);
		count += sizeof(int);
		
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);
		
		count += sizeof(ushort);
		count += chatLen;
		

        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;

        return SendBuffHelper.Close(count);
    }
}

