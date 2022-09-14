using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DummyClient
{


    class ServerSession : Session
    {
       
        //서버 전담 대리자(?)
        public override void OnConnected(EndPoint endpoint)
        {

            Console.WriteLine($"On Connected");
            
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
