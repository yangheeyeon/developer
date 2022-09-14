
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

interface IPacket
{
	public ushort Protocol { get; }
	public void Read(ArraySegment<byte> segment);
	public ArraySegment<byte> Write();
}

public enum PacketID
{
    
C_Chat = 1,
		
S_Chat = 2,
		

}


public class C_Chat : IPacket
{

    
	public string chat;
	

    public ushort Protocol {get { return (ushort)PacketID.C_Chat; } } 

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);//size
        count += sizeof(ushort);//패킷 타입

        
		ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
		count += chatLen;
		
    }

    public ArraySegment<byte> Write()
    {
            
        ArraySegment<byte> segment = SendBuffHelper.Open(1024);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

           
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_Chat);
        count += sizeof(ushort);

        
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);
		count += sizeof(ushort);
		count += chatLen;
		

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;

        return SendBuffHelper.Close(count);

           
    }
}

public class S_Chat : IPacket
{

    
	public int playerId;
	
	
	public string chat;
	

    public ushort Protocol {get { return (ushort)PacketID.S_Chat; } } 

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);//size
        count += sizeof(ushort);//패킷 타입

        
		playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
		count += chatLen;
		
    }

    public ArraySegment<byte> Write()
    {
            
        ArraySegment<byte> segment = SendBuffHelper.Open(1024);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

           
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_Chat);
        count += sizeof(ushort);

        
		success &= BitConverter.TryWriteBytes(s.Slice(count , s.Length - count), playerId);
		count += sizeof(int);
		
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);
		count += sizeof(ushort);
		count += chatLen;
		

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;

        return SendBuffHelper.Close(count);

           
    }
}


