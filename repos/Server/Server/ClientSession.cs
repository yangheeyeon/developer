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

        C_PlayerInfoReq p = new C_PlayerInfoReq() { playerId = 1001 };
        public override void OnConnected(EndPoint endpoint)
        {
            /*ArraySegment<byte> openSegment = SendBuffHelper.Open(4096);
            //Base data type => 바이트 배열
            byte[] buff = BitConverter.GetBytes(packet.size);
            byte[] buff2 = BitConverter.GetBytes(packet.packetId);

            Array.Copy(buff, 0, openSegment.Array, openSegment.Offset, buff.Length);
            Array.Copy(buff2, 0, openSegment.Array, openSegment.Offset + buff.Length, buff2.Length);

            ArraySegment<byte> sendBuff = SendBuffHelper.Close(packet.size);

            Send(sendBuff);*/

            Console.WriteLine($"OnConnected : {endpoint}");
            Thread.Sleep(1000);

            Disconnect();


        }
        public override void OnRecvPacket(ArraySegment<byte> buff)
        {
            //swich = 패킷 타입 찾는 시간 오래 걸림!
            PacketManager.Instance.OnRecvPacket(this, buff);
        }

        public override void OnDisconnected(EndPoint endpoint)
        {
            Console.WriteLine($"OnDisconnected {endpoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"TransferredBytes {numOfBytes}");
        }


        
    }
}
