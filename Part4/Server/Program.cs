using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Program
    {
        
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();
        static void Main(String[] args)
        {
           

            //네트워크 연결 준비
            //DNS(Domain Name System)
            string hostName = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(hostName);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            
            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });

            while (true)
            {
                //GameRoom Flush수행
                Room.Push(()=>Room.Flush());
                Thread.Sleep(250);
            }



        }
    }
}
