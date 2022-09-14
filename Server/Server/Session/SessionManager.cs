using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    //클라 세션 생성 or 제거 에 관여
    class SessionManager
    {
        //싱글톤 패턴
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        int _sessionId = 0;

        
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        Object _lock = new Object();

        public ClientSession Generate()
        {
            lock (_lock)
            {
                ClientSession session = new ClientSession();
                int id = ++_sessionId;
                session.SessionId = id;
                _sessions.Add(id, session);

                Console.WriteLine($"clientSession Connected to {id}");
                return session;
            }
            
            
        }
        public ClientSession Find(int id)
        {
            lock (_lock)
            {
                
                ClientSession session = null;
                _sessions.TryGetValue(id, out session);
                //없으면 null반환
                return session;
            }
            
        }
        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionId);
            }
        }
    }
}
