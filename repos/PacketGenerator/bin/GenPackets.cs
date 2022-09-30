
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

enum PacketId
{
    
PlayerInfoReq = 1,

}

interface IPacket
{
	ushort Protocol { get;}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

    
 public class PlayerInfoReq :IPacket
{
    
	    public long playerid;
	
	
	
	
	    public string name;
	
	
	
	
	public List<Skill> skills = new List<Skill>();
	
	public struct Skill
	{
	    
		    public int id;
		
		
		
		
		    public short level;
		
		
		
		
		    public float duration;
		
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        
			this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
			count += sizeof(short);
			
			this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
	    }
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	                    
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
			count += sizeof(short);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
			count += sizeof(float);
			
	
	
	        return success;
	    }
	
	                
	}
	
    
    public ushort Protocol { get{ return (ushort)PacketId.PlayerInfoReq ; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);


        count += 2;//size
        count += 2;//packetid

        
		this.playerid = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		
		ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += nameLen;
		
		ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		skills.Clear();
		
		for (int i = 0; i < skillLen; i++)
		{
		    Skill skill = new Skill();
		    skill.Read(s, ref count);
		    skills.Add(skill);
		}
		

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketId.PlayerInfoReq);
        count += sizeof(ushort);

        
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerid);
		count += sizeof(long);
		
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
		
		count += sizeof(ushort);
		count += nameLen;
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
		count += sizeof(ushort);
		
		foreach (Skill skill in skills)
		        skill.Write(s, ref count);
		

        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;

        return SendBuffHelper.Close(count);
    }
}

