using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;


class PacketHandler
{
    static object _lock = new object();
    public static void c_chatHandler(PacketSession session,IPacket packet)
    {

        c_chat cPacket = packet as c_chat;
        ServerSession serverSession = session as ServerSession;

        string cchat = cPacket.chat;
        Console.WriteLine(cchat);
        
        if (serverSession.Room == null)
            return;

        lock (_lock)
        {
            GameRoom room = serverSession.Room;
            room.Push(() => room.Broadcast(serverSession, cchat));
        }
        
    }
}

