using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Server
{
    
    class ServerSession : PacketSession
    {
        public int Sessionid { get; set; }
        public GameRoom Room { get; set; }

        //On~~ : ~~가 완료됨!
        public override void OnConnected(EndPoint endPoint)
        {
            //연결됨
            Console.WriteLine($"OnConnected to {endPoint}");

            //Program.Room에 입장
            Program.Room.Push(() => Program.Room.Enter(this));

            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected to {endPoint}");
            if(Room != null)
            {
                GameRoom room = Room;
                room.Push(() => room.Leave(this));
                Room = null;
            }
            
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {

            PacketManager.Instance.OnRecvPacket(this, buffer);
            
            
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred Bytes:{numOfBytes}");
        }
    }
}
