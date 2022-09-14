
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
    
C_PlayerInfoReq = 1,
		

}


public class C_PlayerInfoReq : IPacket
{

    
	public byte testByte;
	
	
	public long playerId;
	
	
	public string name;
	
	
	public List<Skill> skills = new List<Skill>();
	
	public struct Skill
	{
	    
		public int id;
		
		
		public ushort level;
		
		
		public float duration;
		
	
	    public void Read(Span<byte> s, ref ushort count)
	    {
	        
			id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			level = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
			count += sizeof(ushort);
			
			duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
	
	
	    }
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count , s.Length - count), id);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count , s.Length - count), level);
			count += sizeof(ushort);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count , s.Length - count), duration);
			count += sizeof(float);
			
	
	        return success;
	    }
	         
	}
	

    public ushort Protocol {get { return (ushort)PacketID.C_PlayerInfoReq; } } 

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);//size
        count += sizeof(ushort);//패킷 타입

        
		testByte = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
		
		playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		
		ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += nameLen;
		
		 ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		skills.Clear();
		for(int i = 0; i< skillLen; i++)
		{
		    Skill skill = new Skill();
		    skill.Read(s, ref count);
		    skills.Add(skill);
		}
		
    }

    public ArraySegment<byte> Write()
    {
            
        ArraySegment<byte> segment = SendBuffHelper.Open(1024);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

           
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_PlayerInfoReq);
        count += sizeof(ushort);

        
		segment.Array[segment.Offset + count] = (byte)this.testByte;
		count += sizeof(byte);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count , s.Length - count), playerId);
		count += sizeof(long);
		
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
		count += sizeof(ushort);
		count += nameLen;
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
		count += sizeof(ushort);
		
		foreach(Skill skill in skills)
		{
		    success &= skill.Write(s, ref count);
		}
		

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;

        return SendBuffHelper.Close(count);

           
    }
}


