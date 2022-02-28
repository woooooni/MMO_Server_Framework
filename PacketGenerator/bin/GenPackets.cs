using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;

public enum PacketID
{
    PlayerInfoReq = 1,
	Test = 2,
	
}

interface IPacket
{
	ushort protocol { get; }
	void Read(ArraySegment<byte> seg);
	ArraySegment<byte> Write();
}


class PlayerInfoReq : IPacket
{
    public byte testByte;
	public long playerID;
	public string name;
	public class Skill
	{
	    public int id;
		public short level;
		public float duration;
		public class Att
		{
		    public int att;
		    public void Read(ReadOnlySpan<byte> s, ref ushort count)
		    {
		        this.att = BitConverter.ToInt32(s.Slice(count, s.Length - count));
				count += sizeof(int);
		    }
		
		    public bool Write(Span<byte> s, ref ushort count)
		    {
		        bool success = true;
		        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.att);
				count += sizeof(int);
		        return success;
		    }
		
		    
		}
		public List<Att> atts = new List<Att>();
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
			count += sizeof(short);
			this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			this.atts.Clear();
			ushort attLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
			count += sizeof(ushort);
			for(int i=0; i<attLen; i++)
			{
			    Att att = new Att();
			    att.Read(s, ref count);
			    atts.Add(att);
			}
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
			count += sizeof(int);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
			count += sizeof(short);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
			count += sizeof(float);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.atts.Count);
			count += sizeof(ushort);
			foreach(Att att in this.atts)
			    success &= att.Write(s, ref count);
	        return success;
	    }
	
	    
	}
	public List<Skill> skills = new List<Skill>();

public ushort protocol { get { return (ushort)PacketID.PlayerInfoReq; } }

    public  void Read(ArraySegment<byte> seg)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(seg.Array, seg.Offset, seg.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.testByte = (byte)seg.Array[seg.Offset + count];
		count += sizeof(byte);
		this.playerID = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += nameLen;
		this.skills.Clear();
		ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		for(int i=0; i<skillLen; i++)
		{
		    Skill skill = new Skill();
		    skill.Read(s, ref count);
		    skills.Add(skill);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(seg.Array, seg.Offset, seg.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq);
        count += sizeof(ushort);
        seg.Array[seg.Offset + count] = (byte)this.testByte;
		count += sizeof(byte);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerID);
		count += sizeof(long);
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, seg.Array, seg.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
		count += sizeof(ushort);
		count += nameLen;
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.skills.Count);
		count += sizeof(ushort);
		foreach(Skill skill in this.skills)
		    success &= skill.Write(s, ref count);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;

        return SendBufferHelper.Close(count);
    }
}


class Test : IPacket
{
    public int testInt;

public ushort protocol { get { return (ushort)PacketID.Test; } }

    public  void Read(ArraySegment<byte> seg)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(seg.Array, seg.Offset, seg.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.testInt = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(seg.Array, seg.Offset, seg.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.Test);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.testInt);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;

        return SendBufferHelper.Close(count);
    }
}


