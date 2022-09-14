using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DummyClient
{
 

    class ServerSession :Session
    {
        //서버 전담 대리자(?)
        public override void OnConnected(EndPoint endpoint)
        {

            C_PlayerInfoReq packet = new C_PlayerInfoReq() { playerId = 1001, name ="ABCD", testByte = 100 };

            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 201, level = 2, duration = 4.0f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 301, level = 3, duration = 5.0f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 401, level = 4, duration = 6.0f });
            //보낸다
            ArraySegment<byte> sendBuff = packet.Write();
            if (sendBuff != null)
                Send(sendBuff);
            

        }

        public override void OnDisconnected(EndPoint endpoint)
        {
            Console.WriteLine($"OnDisconnected {endpoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"TransferredBytes {numOfBytes}");
        }

        public override int OnRecv(ArraySegment<byte> buff)
        {

            string recvData = Encoding.UTF8.GetString(buff.Array, buff.Offset, buff.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buff.Count;
        }
        
    }
}
