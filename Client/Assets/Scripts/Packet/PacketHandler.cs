using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class PacketHandler
{
    //추가할 때는 용도 구분을 명확하게 하자(보안)
    public static void S_BroadCastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadCastEnterGame pkt = packet as S_BroadCastEnterGame;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.EnterGame(pkt);
    }

    public static void S_BroadCastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadCastLeaveGame pkt = packet as S_BroadCastLeaveGame;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.LeaveGame(pkt);
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList pkt = packet as S_PlayerList;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Add(pkt);
    }

    public static void S_BroadCastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadCastMove pkt = packet as S_BroadCastMove;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Move(pkt);
    }
}

