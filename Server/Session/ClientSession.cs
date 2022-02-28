using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;

namespace Server
{
	class ClientSession : PacketSession
    {
        public int SessionID { get; set; }
        public GameRoom Room { get; set; }
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            //Packet packet = new Packet(){size = 100, packetID = 10};

            //보낸다
            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer1 = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetID);
            //Array.Copy(buffer1, 0, openSegment.Array, 0, buffer1.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer1.Length + buffer2.Length);

            // ex) 100명
            //      1 -> 이동 패킷이 100명
            //      100 -> 이동 패킷이 100 * 100 = 1만개가 전송되어야 함.

            //Send(sendBuff);
            //클라이언트 쪽에서도 로딩이 끝났는지 체크하여 다음 동작을 구현해야 합니다.
            //Thread.Sleep(5000);
            //Disconnect();

            Program.Room.Push(()=> Program.Room.Enter(this));
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room != null)
            {
                GameRoom room = Room;
                room.Push(() => room.Leave(this));
                Room = null;
            }

            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
