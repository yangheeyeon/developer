using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Server
{
    class ClientSession : PacketSession
    {
        //클라이언트 전담 대리자(?)
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }
        public override void OnConnected(EndPoint endpoint)
        {
            

            Console.WriteLine($"OnConnected : {endpoint}");
            //바로 방에다 집어 넣기
            Program.Room.Enter(this);


        }
        public override void OnRecvPacket(ArraySegment<byte> buff)
        {
            //swich = 패킷 타입 찾는 시간 오래 걸림!
            PacketManager.Instance.OnRecvPacket(this, buff);
        }

        public override void OnDisconnected(EndPoint endpoint)
        {
            //세션 리스트에서 제거
            SessionManager.Instance.Remove(this);
            if(this.Room != null)
            {
                this.Room.Leave(this);
                this.Room = null;
            }
            Console.WriteLine($"OnDisconnected {endpoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"TransferredBytes {numOfBytes}");
        }


        
    }
}
