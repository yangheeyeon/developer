using ServerCore;
using System;
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
            
            //Dns
            string host = Dns.GetHostName();
            IPHostEntry IPHost = Dns.GetHostEntry(host);
            IPAddress IPAddr = IPHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(IPAddr, 7777);

            //입장 문의
            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return new ServerSession(); });

            while (true) { 

                try
                {
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(100);


            }


        }
    }
}
