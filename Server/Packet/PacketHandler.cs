﻿using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {//어떤 세션에서 조립됐나? // 어떤 패킷인가?
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Leave(clientSession)
        );
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {//어떤 세션에서 조립됐나? // 어떤 패킷인가?
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        //Console.WriteLine($"{movePacket.posX}, {movePacket.posY}, {movePacket.posZ}");

        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Move(clientSession, movePacket)
        );
    }
}

