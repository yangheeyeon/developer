using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ServerSession> _sessions = new List<ServerSession>();

        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();


        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }
        public void Flush()
        {
            foreach (ServerSession session in _sessions)
                session.Send(_pendingList);

            Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }
        public void Enter(ServerSession session)
        {
          
            _sessions.Add(session);
            session.Room = this;
            
        }
        public void Leave(ServerSession session)
        {
            _sessions.Remove(session);
           
        }
        public void Broadcast(ServerSession session,string chat )
        {
            s_chat spacket = new s_chat();
            spacket.playerid = session.Sessionid;
            spacket.chat = $"{chat} i am {session.Sessionid}";
            
            ArraySegment<byte> segment = spacket.Write();
            try
            {
                _pendingList.Add(segment);

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
           
        }

       
    }
}
