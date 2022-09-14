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

        static void Main(string[] args)
        {
            //딕셔너리 초기화
            PacketManager.Instance.Register();

            //DNS
            string host = Dns.GetHostName();
            IPHostEntry IPHost = Dns.GetHostEntry(host);
            IPAddress IPAddr = IPHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(IPAddr, 7777);


            //멀리있는 문지기 호출
            _listener.Init(endPoint, () => { return new ClientSession(); });


            Console.WriteLine("Listening...");

            while (true)
            {
            }





        }
    }
}
