using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {//어떤 세션에서 조립됐나? // 어떤 패킷인가?
        C_Chat chatPacket = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Broadcast(clientSession, chatPacket.chat)
        );
    }
}

