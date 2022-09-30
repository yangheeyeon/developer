using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class SessionManager
    {

        static SessionManager _instance = new SessionManager();
        public static SessionManager Instance{ get { return _instance; } }

        int sessionid = 0;
        Dictionary<int, ServerSession> _sessions = new Dictionary<int, ServerSession>();

        object _lock = new object();

        public ServerSession Generate()
        {
            lock (_lock)
            {
                int _sessionid = ++sessionid;

                ServerSession _session = new ServerSession();
                _session.Sessionid = _sessionid;
                _sessions.Add(_sessionid, _session);

                Console.WriteLine($"Generated : {_sessionid}");

                return _session;
            }
           
        }
        public ServerSession Find(int id)
        {
            lock (_lock)
            {
                ServerSession _session = null;
                _sessions.TryGetValue(id, out _session);

                return _session;
            }
        }
        public void Remove(ServerSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.Sessionid);
            }
        }
    }
}
