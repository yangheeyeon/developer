using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    
    class GameRoom
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        Object _lock = new Object();
        public void Broadcast(ClientSession clientSession,string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = clientSession.SessionId;
            packet.chat = chat;
            //패킷 타입, 클라이언트 id, 문자열 길이, 문자열 내용 
            ArraySegment<byte> segment = packet.Write();
            //방안의 모든 사람에게 보내기
            lock (_lock)
            {
                foreach(ClientSession s in _sessions)
                {
                    s.Send(segment);
                }
            }


        }
        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
            
        }
        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
        }
    }
}
