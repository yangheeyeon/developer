using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;
       public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            //Socket()
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory = sessionFactory;

            //Bind()
            _listenSocket.Bind(endPoint);

            //Listen(대기수)
            _listenSocket.Listen(10);
            //call-back 방식
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);


        }
        public void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending =_listenSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(null, args);
        }
        public void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            //call-back방식
            if(args.SocketError == SocketError.Success)
            {
           
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }
            //call(하면)back(할께~!)방식
            RegisterAccept(args);

           
        }


    }
}
