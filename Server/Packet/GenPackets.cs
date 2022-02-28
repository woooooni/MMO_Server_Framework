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
    S_BroadCastEnterGame = 1,
	C_LeaveGame = 2,
	S_BroadCastLeaveGame = 3,
	S_PlayerList = 4,
	C_Move = 5,
	S_BroadCastMove = 6,
	
}

public interface IPacket
{
	ushort protocol { get; }
	void Read(ArraySegment<byte> seg);
	ArraySegment<byte> Write();
}


public class S_BroadCastEnterGame : IPacket
{
    public int playerID;
	public float posX;
	public float posY;
	public float posZ;

public ushort protocol { get { return (ushort)PacketID.S_BroadCastEnterGame; } }

    public  void Read(ArraySegment<byte> seg)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerID = BitConverter.ToInt32(seg.Array, seg.Offset + count);
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(seg.Array, seg.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(seg.Array, seg.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(seg.Array, seg.Offset + count);
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastEnterGame), 0, seg.Array, seg.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerID), 0, seg.Array, seg.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, seg.Array, seg.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, seg.Array, seg.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, seg.Array, seg.Offset + count, sizeof(float));
		count += sizeof(float);

        Array.Copy(BitConverter.GetBytes(count), 0, seg.Array, seg.Offset, sizeof(ushort));
        
        return SendBufferHelper.Close(count);
    }
}


public class C_LeaveGame : IPacket
{
    

public ushort protocol { get { return (ushort)PacketID.C_LeaveGame; } }

    public  void Read(ArraySegment<byte> seg)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveGame), 0, seg.Array, seg.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, seg.Array, seg.Offset, sizeof(ushort));
        
        return SendBufferHelper.Close(count);
    }
}


public class S_BroadCastLeaveGame : IPacket
{
    public int playerID;

public ushort protocol { get { return (ushort)PacketID.S_BroadCastLeaveGame; } }

    public  void Read(ArraySegment<byte> seg)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerID = BitConverter.ToInt32(seg.Array, seg.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastLeaveGame), 0, seg.Array, seg.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerID), 0, seg.Array, seg.Offset + count, sizeof(int));
		count += sizeof(int);

        Array.Copy(BitConverter.GetBytes(count), 0, seg.Array, seg.Offset, sizeof(ushort));
        
        return SendBufferHelper.Close(count);
    }
}


public class S_PlayerList : IPacket
{
    public class Player
	{
	    public bool isSelf;
		public int playerID;
		public float posX;
		public float posY;
		public float posZ;
	    public void Read(ArraySegment<byte> seg, ref ushort count)
	    {
	        this.isSelf = BitConverter.ToBoolean(seg.Array, seg.Offset + count);
			count += sizeof(bool);
			this.playerID = BitConverter.ToInt32(seg.Array, seg.Offset + count);
			count += sizeof(int);
			this.posX = BitConverter.ToSingle(seg.Array, seg.Offset + count);
			count += sizeof(float);
			this.posY = BitConverter.ToSingle(seg.Array, seg.Offset + count);
			count += sizeof(float);
			this.posZ = BitConverter.ToSingle(seg.Array, seg.Offset + count);
			count += sizeof(float);
	    }
	
	    public bool Write(ArraySegment<byte> seg, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(this.isSelf), 0, seg.Array, seg.Offset + count, sizeof(bool));
			count += sizeof(bool);
			Array.Copy(BitConverter.GetBytes(this.playerID), 0, seg.Array, seg.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.posX), 0, seg.Array, seg.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.posY), 0, seg.Array, seg.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.posZ), 0, seg.Array, seg.Offset + count, sizeof(float));
			count += sizeof(float);
	        return success;
	    }
	
	    
	}
	public List<Player> players = new List<Player>();

public ushort protocol { get { return (ushort)PacketID.S_PlayerList; } }

    public  void Read(ArraySegment<byte> seg)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(seg.Array, seg.Offset + count);
		count += sizeof(ushort);
		for(int i=0; i<playerLen; i++)
		{
		    Player player = new Player();
		    player.Read(seg, ref count);
		    players.Add(player);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_PlayerList), 0, seg.Array, seg.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.players.Count), 0, seg.Array, seg.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach(Player player in this.players)
		    player.Write(seg, ref count);

        Array.Copy(BitConverter.GetBytes(count), 0, seg.Array, seg.Offset, sizeof(ushort));
        
        return SendBufferHelper.Close(count);
    }
}


public class C_Move : IPacket
{
    public float posX;
	public float posY;
	public float posZ;

public ushort protocol { get { return (ushort)PacketID.C_Move; } }

    public  void Read(ArraySegment<byte> seg)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.posX = BitConverter.ToSingle(seg.Array, seg.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(seg.Array, seg.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(seg.Array, seg.Offset + count);
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Move), 0, seg.Array, seg.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.posX), 0, seg.Array, seg.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, seg.Array, seg.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, seg.Array, seg.Offset + count, sizeof(float));
		count += sizeof(float);

        Array.Copy(BitConverter.GetBytes(count), 0, seg.Array, seg.Offset, sizeof(ushort));
        
        return SendBufferHelper.Close(count);
    }
}


public class S_BroadCastMove : IPacket
{
    public int playerID;
	public float posX;
	public float posY;
	public float posZ;

public ushort protocol { get { return (ushort)PacketID.S_BroadCastMove; } }

    public  void Read(ArraySegment<byte> seg)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerID = BitConverter.ToInt32(seg.Array, seg.Offset + count);
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(seg.Array, seg.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(seg.Array, seg.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(seg.Array, seg.Offset + count);
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastMove), 0, seg.Array, seg.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerID), 0, seg.Array, seg.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, seg.Array, seg.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, seg.Array, seg.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, seg.Array, seg.Offset + count, sizeof(float));
		count += sizeof(float);

        Array.Copy(BitConverter.GetBytes(count), 0, seg.Array, seg.Offset, sizeof(ushort));
        
        return SendBufferHelper.Close(count);
    }
}


