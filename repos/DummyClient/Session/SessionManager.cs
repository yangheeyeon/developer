using System;
using System.Collections.Generic;
using System.Text;

namespace DummyClient
{
    class SessionManager
    {
        static SessionManager _instance = new SessionManager();
        public static SessionManager Instance { get { return _instance; } }

        List<ClientSession> _sessions = new List<ClientSession>();

        List<ArraySegment<byte>> _pendingLIst = new List<ArraySegment<byte>>();
        object _lock = new object();
        public ClientSession Generate()
        {
            lock (_lock)
            {
                ClientSession clientSession = new ClientSession();
                _sessions.Add(clientSession);
                return clientSession;
            }
   
        }
        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);

            }
        }
        //sendforeach ! -----------------------------------------------------
        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (ClientSession s in _sessions)
                {
                    c_chat chatPacket = new c_chat();
                    chatPacket.chat = $"Hello Server";
                    
                    ArraySegment<byte> segment = chatPacket.Write();
                    s.Send(segment);
                }

            }
        }

    }
}
