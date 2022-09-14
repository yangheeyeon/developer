using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{

    class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();
        static void Main(string[] args)
        {
            //딕셔너리 초기화
            PacketManager.Instance.Register();

            //DNS
            string host = Dns.GetHostName();
            IPHostEntry IPHost = Dns.GetHostEntry(host);
            IPAddress IPAddr = IPHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(IPAddr, 7777);

            //문지기
            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });


            Console.WriteLine("Listening...");

            while (true)
            {
            }





        }
    }
}
