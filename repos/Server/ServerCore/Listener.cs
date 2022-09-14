using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        Func<Session> _sessionFactory;
        
        Socket _listenSocket;
        //문지기 이곳으로 데려옴
        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            //구독
            _sessionFactory += sessionFactory;

            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //문지기 교육
            _listenSocket.Bind(endPoint);

            //영업 시작
            //최대 대기 번호표
            _listenSocket.Listen(10);

            //다른 세계(차원)만들기
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);
        }
        
        public void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            //다른 차원에 일 지시
            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(null, args);
            
        }
        public void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Init(args.AcceptSocket);
                
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());
            //낚시대 던진다
            RegisterAccept(args);
        }

        public Socket Accept()
        {
            return _listenSocket.Accept();
        }
    }
}
