using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }
    #endregion

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
    public void Register()
    {
        //자동화 코드
        _makeFunc.Add((ushort)PacketID.S_BroadCastEnterGame, MakePacket<S_BroadCastEnterGame>);
        _handler.Add((ushort)PacketID.S_BroadCastEnterGame, PacketHandler.S_BroadCastEnterGameHandler);

        _makeFunc.Add((ushort)PacketID.S_BroadCastLeaveGame, MakePacket<S_BroadCastLeaveGame>);
        _handler.Add((ushort)PacketID.S_BroadCastLeaveGame, PacketHandler.S_BroadCastLeaveGameHandler);

        _makeFunc.Add((ushort)PacketID.S_PlayerList, MakePacket<S_PlayerList>);
        _handler.Add((ushort)PacketID.S_PlayerList, PacketHandler.S_PlayerListHandler);

        _makeFunc.Add((ushort)PacketID.S_BroadCastMove, MakePacket<S_BroadCastMove>);
        _handler.Add((ushort)PacketID.S_BroadCastMove, PacketHandler.S_BroadCastMoveHandler);


    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallBack = null)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (_makeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func.Invoke(session, buffer);
            if (onRecvCallBack != null)
                onRecvCallBack.Invoke(session, packet);
            else
                HandlePacket(session, packet);
        }
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read(buffer);

        return packet;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.protocol, out action))
            action.Invoke(session, packet);
    }
}
