using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //네트워크 연결 준비
            //DNS(Domain Name System)
            string hostName = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(hostName);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            
            Connector connector = new Connector();
            //ClientSession x 10 개
  
          
            connector.Connect(
            endPoint,
            () => { return SessionManager.Instance.Generate(); },
         10
            );

           
            //매니저가 서버에 패킷 전송
            while (true)
            {
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                //0.25초 마다 서버에 전송
                Thread.Sleep(250); 
            }
            
        }
    }
}
