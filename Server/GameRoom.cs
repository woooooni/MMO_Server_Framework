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

            Console.WriteLine($"Flushed{_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Broadcast(ClientSession session, string chat)
        {
            //세션을 받아 패킷을 만들어주는 부분이라 lock 처리 안해도 됨. 다른쓰레드랑 공유하고 있지 않음.
            S_Chat packet = new S_Chat();
            packet.playerID = session.SessionID;
            packet.chat = $"{chat} I AM {packet.playerID}";
            ArraySegment<byte> seg = packet.Write();

            _pendingList.Add(seg);
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
            _sessions.Add(session);
            session.Room = this;
        }
        public void Leave(ClientSession session)
        {
             _sessions.Remove(session);
        }
    }
}
