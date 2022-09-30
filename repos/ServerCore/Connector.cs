using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Connector
    {
        //연결 요청하는 기능!
        Func<Session> _sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
        {
            //여러개 소켓
            for (int i = 0; i < count; i++)
            {
                _sessionFactory = sessionFactory;

                Socket _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
                args.RemoteEndPoint = endPoint;

                args.UserToken = _socket;

                RegisterConnect(args);
            }


        }
        public void RegisterConnect(SocketAsyncEventArgs args)
        {
            try
            {
                //요청하는 측 소켓 꺼내기!
                Socket socket = args.UserToken as Socket;
                if (socket == null)
                    return;
                bool pending = socket.ConnectAsync(args);
                if (pending == false)
                    OnConnectCompleted(null, args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {

                Session session = _sessionFactory.Invoke();
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted Fail {args.SocketError}");
            }
        }
    }
}
