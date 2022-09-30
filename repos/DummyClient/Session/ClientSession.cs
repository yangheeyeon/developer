using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace DummyClient
{
    //disconnect 언제함?
    class ClientSession : PacketSession
    {
       
		//On~~ : ~~가 완료됨!
		public override void OnConnected(EndPoint endPoint)
        {
            //연결됨
            Console.WriteLine($"OnConnected to {endPoint}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected to {endPoint}");
            SessionManager.Instance.Remove(this);
           
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
            
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred Bytes:{numOfBytes}");
        }
    }
}
