using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;


class PacketHandler
{
    public static void s_chatHandler(PacketSession session,IPacket packet)
    {
        //무슨 패킷을 누가 처음 받았는지 
        s_chat chatPacket = packet as s_chat;
        ClientSession _session = session as ClientSession;

        string chat = chatPacket.chat;
        int len = chat.Length;
  
        Console.WriteLine($"{chat}");
            

    }
}

