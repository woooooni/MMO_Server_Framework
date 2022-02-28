using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameRoom : IJobQueue
    {
        //게임룸에선 모든게 멀티쓰레드 환경에서 돌아간다고 생각하자!
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (ClientSession s in _sessions)
              s.Send(_pendingList);

            //Console.WriteLine($"Flushed{_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
            // N ^ 2 개선
            // foreach로 모든 세션에 뿌려주면 부담된다(N^2) 
            // 패킷을 받으면 바로 보내는 것이 아니라, 패킷을 뭉쳐서 보내면 N까지 줄일 수 있음.
            // 1. Core단에서 모아보내거나,
            // 2. Contents단에서 모아보내면 됨.
            //foreach (ClientSession s in _sessions)
            //  s.Send(seg);
        }


        public void Enter(ClientSession session)
        {
            // 플레이어 추가 부분
            _sessions.Add(session);
            session.Room = this;

            // 신입한테 모든 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();
            foreach(ClientSession s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerID = s.SessionID,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ
                });
            }
            session.Send(players.Write());

            // 신입 입장을 모두에게 알린다
            S_BroadCastEnterGame enter = new S_BroadCastEnterGame();
            enter.playerID = session.SessionID;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;
            Broadcast(enter.Write());
        }
        public void Leave(ClientSession session)
        {
            //플레이어 제거
            _sessions.Remove(session);

            //모두에게 알린다
            S_BroadCastLeaveGame leave = new S_BroadCastLeaveGame();
            leave.playerID = session.SessionID;
            Broadcast(leave.Write());
        }

        public void Move(ClientSession session, C_Move packet)
        {
            // 좌표 바꾸고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            // 모두에게 알린다.
            S_BroadCastMove move = new S_BroadCastMove();
            move.playerID = session.SessionID;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;
            Broadcast(move.Write());
        }
    }
}
