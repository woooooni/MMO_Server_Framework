﻿using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    //추가할 때는 용도 구분을 명확하게 하자(보안)
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat chatPacket = packet as S_Chat; ;
        ServerSession serverSession = session as ServerSession;

        //if(chatPacket.playerID == 1)
            //Console.WriteLine(chatPacket.chat);
    }
}

